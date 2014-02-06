using System;
using System.Reactive.Linq;
using Adaptive.ReactiveTrader.Contracts;
using log4net;

namespace Adaptive.ReactiveTrader.Client.Transport
{
    internal class SampleClient : ISampleClient
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (SampleClient));
        private readonly ICurrencyPairRepository _currencyPairRepository;
        private readonly IExecutionServiceClient _executionServiceClient;
        private readonly ISpotStreamRepository _spotStreamRepository;
        private readonly ITradeRepository _tradeRepository;
        private readonly ITransport _transport;

        public SampleClient(
            ITransport transport,
            ISpotStreamRepository spotStreamRepository,
            IExecutionServiceClient executionServiceClient,
            ITradeRepository tradeRepository,
            ICurrencyPairRepository currencyPairRepository)
        {
            _transport = transport;
            _spotStreamRepository = spotStreamRepository;
            _executionServiceClient = executionServiceClient;
            _tradeRepository = tradeRepository;
            _currencyPairRepository = currencyPairRepository;
        }

        public void Start()
        {
            _transport.TransportStatuses.Subscribe(
                status =>
                {
                    switch (status)
                    {
                        case TransportStatus.Connecting:
                            // dispay some visual clue that the app is loading
                            break;
                        case TransportStatus.Connected:
                            // OK we are connected
                            break;
                        case TransportStatus.ConnectionSlow:
                            // network issue, we are likely disconnected at this stage. Notify user that the connection has an issue
                            // this happens when we have missed some keep alive but SignalR has not yet decided that the trasnport was disconnected
                            break;
                        case TransportStatus.Reconnecting:
                            // we've lost the connection and SignalR is trying to reconnect automatically
                            break;
                        case TransportStatus.Reconnected:
                            // SignalR managed to reconnect to the server
                            break;
                        case TransportStatus.Closed:
                            // SignalR stopped reconnecting. It is now our responsibility to restart the transport, potentially with another server.
                            break;
                    }
                });

            // dispose transportSubscription to stop the transport
            IDisposable transportSubscription = _transport.Initialize("http://localhost:8080", "Olivier")
                .Subscribe(_ =>
                {
                    Log.Info("Transport initialized");

                    Run();
                },
                    ex => Log.Error("An error occured within SignalR transport.", ex),
                    () => Log.Info("Transport status stream completed, it was closed."));
        }

        private async void Run()
        {
            await _currencyPairRepository.Initialize();

            _tradeRepository.GetAllTrades()
                .Subscribe(
                    t => Log.InfoFormat("Trade received: {0}", t),
                    ex => Log.Error("An error occured within the blotter stream", ex),
                    () => Log.InfoFormat("Blotter stream completed"));

            foreach (CurrencyPair currencyPair in _currencyPairRepository.GetAllCurrencyPairs())
            {
                string symbol = currencyPair.Symbol;

                if (symbol == "EURGBP")
                {
                    _spotStreamRepository.GetSpotStream("EURGBP")
                        .Do(p => Log.InfoFormat("New price received: {0}", p))
                        .Skip(3)
                        .Take(1)
                        .Subscribe(async p =>
                        {
                            var spotTradeRequest = new SpotTradeRequest
                            {
                                Direction = Direction.Buy,
                                Notional = 1000000,
                                Price = p
                            };

                            try
                            {
                                Log.InfoFormat("Executing on price {0}", p);
                                SpotTrade trade = await _executionServiceClient.Execute(spotTradeRequest);
                                Log.InfoFormat("Execution complexted: {0}", trade);
                            }
                            catch (Exception e)
                            {
                                Log.Error("An error occured when trying to execute trade request", e);
                            }
                        },
                            ex => Log.Error("An error occured within EURGBP stream", ex),
                            () => Log.InfoFormat("EURGBP stream completed"));
                }
                else
                {
                    _spotStreamRepository.GetSpotStream(symbol)
                        .Subscribe(p => Log.InfoFormat("New price received: {0}", p),
                            ex => Log.Error(string.Format("An error occured within {0} stream", symbol), ex),
                            () => Log.InfoFormat(symbol + " stream completed"));
                }
            }
        }
    }
}
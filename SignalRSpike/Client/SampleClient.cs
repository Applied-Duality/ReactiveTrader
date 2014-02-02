using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Dto;
using log4net;

namespace Client
{
    class SampleClient : ISampleClient
    {
        private readonly ITransport _transport;
        private readonly ISpotStreamRepository _spotStreamRepository;
        private readonly IExecutionServiceClient _executionServiceClient;
        private readonly ITradeRepository _tradeRepository;
        private readonly ICurrencyPairRepository _currencyPairRepository;

        private static readonly ILog Log = LogManager.GetLogger(typeof(SampleClient));

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

        public async Task Start()
        {
            try
            {
                await _transport.Initialize("http://localhost:8080", "Olivier");
            }
            catch (Exception e)
            {
                Log.Fatal("An error occured while initializing Transport", e);
                throw;
            }

            await _currencyPairRepository.Initialize();

            _tradeRepository.GetAllTrades()
                .Subscribe(
                    t => Log.InfoFormat("Trade received: {0}", t),
                    ex => Log.Error("An error occured within the blotter stream", ex),
                    () => Log.InfoFormat("Blotter stream completed"));

            foreach (var currencyPair in _currencyPairRepository.GetAllCurrencyPairs())
            {
                var symbol = currencyPair.Symbol;

                if (symbol == "EURGBP")
                {
                    _spotStreamRepository.GetSpotStream("EURGBP")
                        .Do(p => Log.InfoFormat((string) "New price received: {0}", (object) p))
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
                                var trade = await _executionServiceClient.Execute(spotTradeRequest);
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
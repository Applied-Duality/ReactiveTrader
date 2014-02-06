using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Adaptive.ReactiveTrader.Contracts;
using log4net;
using Microsoft.AspNet.SignalR.Client;

namespace Adaptive.ReactiveTrader.Client.Transport
{
    class TradeRepository : ITradeRepository
    {
        private readonly ITransport _transport;
        private readonly IObservable<SpotTrade> _allTrades;
        private static readonly ILog Log = LogManager.GetLogger(typeof(TradeRepository));

        public TradeRepository(ITransport transport)
        {
            _transport = transport;

            _allTrades = Observable.Create<SpotTrade>(observer => _transport.BlotterHubProxy.On<SpotTrade>(ServiceConstants.Client.OnNewTrade, observer.OnNext))
                .Publish()
                .RefCount();
        }

        public IObservable<SpotTrade> GetAllTrades()
        {
            return Observable.Create<SpotTrade>(async observer =>
            {
                var disposables = new CompositeDisposable();

                // subscribe to trade feed first, otherwise there is a race condition 
                disposables.Add(
                    _allTrades.Subscribe(observer));

                // send a subscription request
                try
                {
                    Log.Info("Sending trade subscription...");
                    await _transport.BlotterHubProxy.Invoke(ServiceConstants.Server.SubscribeTrades);
                }
                catch (Exception e)
                {
                    observer.OnError(e);
                }

                disposables.Add(Disposable.Create(async () =>
                {
                    // send unsubscription when the observable gets disposed
                    Log.Info("Sending trades unsubscription...");
                    try
                    {
                        await
                            _transport.BlotterHubProxy.Invoke(ServiceConstants.Server.UnsubscribeTrades);
                    }
                    catch (Exception e)
                    {
                        Log.Error("An error occured while sending trade unsubscription request", e);
                    }
                }));

                return disposables;
            })
                .Publish()
                .RefCount();
        }
    }
}
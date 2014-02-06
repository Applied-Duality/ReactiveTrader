using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Adaptive.ReactiveTrader.Client.Transport;
using Adaptive.ReactiveTrader.Contracts;
using Adaptive.ReactiveTrader.Contracts.Execution;
using log4net;
using Microsoft.AspNet.SignalR.Client;

namespace Adaptive.ReactiveTrader.Client.ServiceClients.Blotter
{
    class BlotterServiceClient : IBlotterServiceClient
    {
        private readonly IHubProxy _blotterHubProxy;

        private static readonly ILog Log = LogManager.GetLogger(typeof(BlotterServiceClient));

        public BlotterServiceClient(ITransport transport)
        {
            _blotterHubProxy = transport.GetProxy(ServiceConstants.Server.BlotterHub);
        }

        public IObservable<IEnumerable<SpotTrade>> GetTrades()
        {
            return Observable.Create<IEnumerable<SpotTrade>>(async observer =>
            {
                // subscribe to trade feed first, otherwise there is a race condition 
                var spotTradeSubscription = _blotterHubProxy.On<IEnumerable<SpotTrade>>(ServiceConstants.Client.OnNewTrade, observer.OnNext);

                // send a subscription request
                try
                {
                    Log.Info("Sending trade subscription...");
                    await _blotterHubProxy.Invoke(ServiceConstants.Server.SubscribeTrades);
                }
                catch (Exception e)
                {
                    observer.OnError(e);
                }

                var unsubscriptionDisposable = Disposable.Create(async () =>
                {
                    // send unsubscription when the observable gets disposed
                    Log.Info("Sending trades unsubscription...");
                    try
                    {
                        await _blotterHubProxy.Invoke(ServiceConstants.Server.UnsubscribeTrades);
                    }
                    catch (Exception e)
                    {
                        Log.Error("An error occured while sending trade unsubscription request", e);
                    }
                });
                return new CompositeDisposable {spotTradeSubscription, unsubscriptionDisposable};
            })
                .Publish()
                .RefCount();
        }
    }
}
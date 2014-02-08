using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Adaptive.ReactiveTrader.Client.Transport;
using Adaptive.ReactiveTrader.Shared;
using Adaptive.ReactiveTrader.Shared.Execution;
using log4net;
using Microsoft.AspNet.SignalR.Client;

namespace Adaptive.ReactiveTrader.Client.ServiceClients.Blotter
{
    class BlotterServiceClient : IBlotterServiceClient
    {
        private readonly IConnectionProvider _connectionProvider;
        private static readonly ILog Log = LogManager.GetLogger(typeof(BlotterServiceClient));

        public BlotterServiceClient(IConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        public IObservable<IEnumerable<TradeDto>> GetTrades()
        {
            return from connection in _connectionProvider.GetActiveConnection().Take(1) // TODO implement recovery when new connection is created.
                from trades in GetTradesForConnection(connection.BlotterHubProxy)
                select trades;
        }

        private IObservable<IEnumerable<TradeDto>> GetTradesForConnection(IHubProxy blotterHubProxy)
        {
            return Observable.Create<IEnumerable<TradeDto>>(async observer =>
            {
                // subscribe to trade feed first, otherwise there is a race condition 
                var spotTradeSubscription = blotterHubProxy.On<IEnumerable<TradeDto>>(ServiceConstants.Client.OnNewTrade, observer.OnNext);

                // send a subscription request
                try
                {
                    Log.Info("Sending trade subscription...");
                    await blotterHubProxy.Invoke(ServiceConstants.Server.SubscribeTrades);
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
                        await blotterHubProxy.Invoke(ServiceConstants.Server.UnsubscribeTrades);
                    }
                    catch (Exception e)
                    {
                        Log.Error("An error occured while sending trade unsubscription request", e);
                    }
                });
                return new CompositeDisposable { spotTradeSubscription, unsubscriptionDisposable };
            })
                .Publish()
                .RefCount();
        } 

    }
}
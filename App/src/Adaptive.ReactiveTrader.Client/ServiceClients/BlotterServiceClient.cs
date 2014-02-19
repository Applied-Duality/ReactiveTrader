using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Adaptive.ReactiveTrader.Client.Transport;
using Adaptive.ReactiveTrader.Shared;
using Adaptive.ReactiveTrader.Shared.Execution;
using log4net;
using Microsoft.AspNet.SignalR.Client;

namespace Adaptive.ReactiveTrader.Client.ServiceClients
{
    class BlotterServiceClient : ServiceClient, IBlotterServiceClient
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(BlotterServiceClient));

        public BlotterServiceClient(IConnectionProvider connectionProvider) : base(connectionProvider)
        {
        }

        public IObservable<IEnumerable<TradeDto>> GetTrades()
        {
            return GetResilientStream(connection => GetTradesForConnection(connection.BlotterHubProxy), TimeSpan.FromSeconds(5));
        }

        private IObservable<IEnumerable<TradeDto>> GetTradesForConnection(IHubProxy blotterHubProxy)
        {
            return Observable.Create<IEnumerable<TradeDto>>(observer =>
            {
                // subscribe to trade feed first, otherwise there is a race condition 
                var spotTradeSubscription = blotterHubProxy.On<IEnumerable<TradeDto>>(ServiceConstants.Client.OnNewTrade, observer.OnNext);

                Log.Info("Sending trade subscription...");
                SendSubscription(blotterHubProxy)
                    .Subscribe(
                        _ => Log.InfoFormat("Subscribed to blotter."),
                        observer.OnError);

                var unsubscriptionDisposable = Disposable.Create(() =>
                {
                    // send unsubscription when the observable gets disposed
                    Log.Info("Sending trades unsubscription...");
                    SendUnsubscription(blotterHubProxy)
                        .Subscribe(
                            _ => Log.InfoFormat("Subscribed to blotter."),
                            ex => Log.Error("An error occured while unsubscribing to blotter.", ex));
                });
                return new CompositeDisposable { spotTradeSubscription, unsubscriptionDisposable };
            })
                .Publish()
                .RefCount();
        }

        private IObservable<Unit> SendSubscription(IHubProxy blotterHubProxy)
        {
            return Observable.FromAsync(() => blotterHubProxy.Invoke(ServiceConstants.Server.SubscribeTrades));
        }

        private IObservable<Unit> SendUnsubscription(IHubProxy blotterHubProxy)
        {
            return Observable.FromAsync(() => blotterHubProxy.Invoke(ServiceConstants.Server.UnsubscribeTrades));
        } 
    }
}
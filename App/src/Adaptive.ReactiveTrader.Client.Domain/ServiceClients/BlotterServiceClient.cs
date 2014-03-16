using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Adaptive.ReactiveTrader.Client.Domain.Transport;
using Adaptive.ReactiveTrader.Shared;
using Adaptive.ReactiveTrader.Shared.DTO.Execution;
using log4net;
using Microsoft.AspNet.SignalR.Client;

namespace Adaptive.ReactiveTrader.Client.Domain.ServiceClients
{
    internal class BlotterServiceClient : ServiceClientBase, IBlotterServiceClient
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(BlotterServiceClient));

        public BlotterServiceClient(IConnectionProvider connectionProvider) : base(connectionProvider)
        {
        }

        public IObservable<IEnumerable<TradeDto>> GetTradesStream()
        {
            return GetResilientStream(connection => GetTradesForConnection(connection.BlotterHubProxy), TimeSpan.FromSeconds(5));
        }

        private static IObservable<IEnumerable<TradeDto>> GetTradesForConnection(IHubProxy blotterHubProxy)
        {
            return Observable.Create<IEnumerable<TradeDto>>(observer =>
            {
                // subscribe to trade feed first, otherwise there is a race condition 
                var spotTradeSubscription = blotterHubProxy.On<IEnumerable<TradeDto>>(ServiceConstants.Client.OnNewTrade, observer.OnNext);

                Log.Info("Sending blotter subscription...");
                var sendSubscriptionDisposable = SendSubscription(blotterHubProxy)
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
                            ex => Log.WarnFormat("An error occured while unsubscribing to blotter: {0}", ex.Message));
                });
                return new CompositeDisposable { spotTradeSubscription, unsubscriptionDisposable, sendSubscriptionDisposable };
            })
                .Publish()
                .RefCount();
        }

        private static IObservable<Unit> SendSubscription(IHubProxy blotterHubProxy)
        {
            return Observable.FromAsync(() => blotterHubProxy.Invoke(ServiceConstants.Server.SubscribeTrades));
        }

        private static IObservable<Unit> SendUnsubscription(IHubProxy blotterHubProxy)
        {
            return Observable.FromAsync(() => blotterHubProxy.Invoke(ServiceConstants.Server.UnsubscribeTrades));
        } 
    }
}
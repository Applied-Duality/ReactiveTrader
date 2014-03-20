using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Adaptive.ReactiveTrader.Client.Domain.Transport;
using Adaptive.ReactiveTrader.Shared;
using Adaptive.ReactiveTrader.Shared.DTO.ReferenceData;
using Adaptive.ReactiveTrader.Shared.Extensions;
using log4net;
using Microsoft.AspNet.SignalR.Client;

namespace Adaptive.ReactiveTrader.Client.Domain.ServiceClients
{
    class ReferenceDataServiceClient : ServiceClientBase, IReferenceDataServiceClient
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ReferenceDataServiceClient));

        public ReferenceDataServiceClient(IConnectionProvider connectionProvider) : base(connectionProvider)
        {
        }

        public IObservable<IEnumerable<CurrencyPairUpdateDto>> GetCurrencyPairUpdatesStream()
        {
            return GetResilientStream(connection => GetTradesForConnection(connection.ReferenceDataHubProxy), TimeSpan.FromSeconds(5));
        }

        private static IObservable<IEnumerable<CurrencyPairUpdateDto>> GetTradesForConnection(IHubProxy referenceDataHubProxy)
        {
            return Observable.Create<IEnumerable<CurrencyPairUpdateDto>>(observer =>
            {
                // subscribe to currency pair update feed first, otherwise there is a race condition 
                var currencyPairUpdateSubscription = referenceDataHubProxy.On<CurrencyPairUpdateDto>(ServiceConstants.Client.OnCurrencyPairUpdate,
                    dto => observer.OnNext(new[] {dto}));

                Log.Info("Sending currency pair subscription...");
                var sendSubscriptionDisposable = GetCurrencyPairUpdatesForConnection(referenceDataHubProxy)
                    .Subscribe(
                        currencyPairs =>
                        {
                            var currencyPairUpdateDtos = currencyPairs as CurrencyPairUpdateDto[] ?? currencyPairs.ToArray();
                            observer.OnNext(currencyPairUpdateDtos);

                            Log.InfoFormat("Subscribed to currency pairs and received {0} currency pairs.", currencyPairUpdateDtos.Count());
                        },
                        observer.OnError);

                var unsubscriptionDisposable = Disposable.Create(() =>
                {
                    // TODO we should add an unsubscription method server side (not a big deal as it gets cleaned-up by signalR
                });
                return new CompositeDisposable { currencyPairUpdateSubscription, unsubscriptionDisposable, sendSubscriptionDisposable };
            })
                .Publish()
                .RefCount();
        }


        private static IObservable<IEnumerable<CurrencyPairUpdateDto>> GetCurrencyPairUpdatesForConnection(IHubProxy referenceDataHubProxy)
        {
            return Observable.FromAsync(
                () => referenceDataHubProxy.Invoke<IEnumerable<CurrencyPairUpdateDto>>(ServiceConstants.Server.GetCurrencyPairs))
                .CacheFirstResult();
        }
    }
}
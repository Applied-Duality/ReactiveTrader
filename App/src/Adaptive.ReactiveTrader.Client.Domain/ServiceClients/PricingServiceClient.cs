using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Adaptive.ReactiveTrader.Client.Domain.Transport;
using Adaptive.ReactiveTrader.Shared;
using Adaptive.ReactiveTrader.Shared.Pricing;
using log4net;
using Microsoft.AspNet.SignalR.Client;

namespace Adaptive.ReactiveTrader.Client.Domain.ServiceClients
{
    class PricingServiceClient : ServiceClient, IPricingServiceClient
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(PricingServiceClient));

        public PricingServiceClient(IConnectionProvider connectionProvider) : base(connectionProvider)
        {
        }

        public IObservable<PriceDto> GetSpotStream(string currencyPair)
        {
            if (string.IsNullOrEmpty(currencyPair)) throw new ArgumentException("currencyPair");

            return GetResilientStream(connection => GetSpotStreamForConnection(currencyPair, connection.PricingHubProxy), TimeSpan.FromSeconds(5));
        }

        private IObservable<PriceDto> GetSpotStreamForConnection(string currencyPair, IHubProxy pricingHubProxy)
        {
            return Observable.Create<PriceDto>(observer =>
            {
                // subscribe to price feed first, otherwise there is a race condition 
                var priceSubscription = pricingHubProxy.On<PriceDto>(ServiceConstants.Client.OnNewPrice, p =>
                {
                    if (p.Symbol == currencyPair)
                    {
                        observer.OnNext(p);
                    } 
                });

                // send a subscription request
                Log.InfoFormat("Sending price subscription for currency pair {0}", currencyPair);
                SendSubscription(currencyPair, pricingHubProxy)
                    .Subscribe(
                        _ => Log.InfoFormat("Subscribed to {0}", currencyPair),
                        observer.OnError);


                var unsubscriptionDisposable = Disposable.Create(() =>
                {
                    // send unsubscription when the observable gets disposed
                    Log.InfoFormat("Sending price unsubscription for currency pair {0}", currencyPair);
                    SendUnsubscription(currencyPair, pricingHubProxy)
                        .Subscribe(
                            _ => Log.InfoFormat("Unsubscribed from {0}", currencyPair),
                            ex =>
                                Log.WarnFormat(
                                    string.Format("An error occured while sending unsubscription request for {0}:{1}",
                                        currencyPair), ex.Message));
                });

                return new CompositeDisposable {priceSubscription, unsubscriptionDisposable};
            })
            .Publish()
            .RefCount();
        }

        private IObservable<Unit> SendSubscription(string currencyPair, IHubProxy pricingHubProxy)
        {
            return
                Observable.FromAsync(
                    () =>
                        pricingHubProxy.Invoke(ServiceConstants.Server.SubscribePriceStream,
                            new PriceSubscriptionRequestDto {CurrencyPair = currencyPair}));
        }

        private IObservable<Unit> SendUnsubscription(string currencyPair, IHubProxy pricingHubProxy)
        {
            return
                Observable.FromAsync(
                    () =>
                        pricingHubProxy.Invoke(ServiceConstants.Server.UnsubscribePriceStream,
                            new PriceSubscriptionRequestDto { CurrencyPair = currencyPair }));
        }
    }
}
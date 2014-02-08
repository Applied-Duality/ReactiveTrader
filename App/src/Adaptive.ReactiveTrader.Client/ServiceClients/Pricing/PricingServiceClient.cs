using System;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Adaptive.ReactiveTrader.Client.Transport;
using Adaptive.ReactiveTrader.Shared;
using Adaptive.ReactiveTrader.Shared.Pricing;
using log4net;
using Microsoft.AspNet.SignalR.Client;

namespace Adaptive.ReactiveTrader.Client.ServiceClients.Pricing
{
    class PricingServiceClient : IPricingServiceClient
    {
        private readonly IConnectionProvider _connectionProvider;

        private static readonly ILog Log = LogManager.GetLogger(typeof(PricingServiceClient));

        public PricingServiceClient(IConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        public IObservable<PriceDto> GetSpotStream(string currencyPair)
        {
            if (string.IsNullOrEmpty(currencyPair)) throw new ArgumentException("currencyPair");

            return from connection in _connectionProvider.GetActiveConnection().Take(1) // TODO handle new connection
                from price in GetSpotStreamForConnection(currencyPair, connection.PricingHubProxy)
                select price;
        }

        public IObservable<PriceDto> GetSpotStreamForConnection(string currencyPair, IHubProxy pricingHubProxy)
        {
            return Observable.Create<PriceDto>(async observer =>
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
                try
                {
                    Log.InfoFormat("Sending price subscription for currency pair {0}", currencyPair);
                    await pricingHubProxy.Invoke(ServiceConstants.Server.SubscribePriceStream, new PriceSubscriptionRequestDto { CurrencyPair = currencyPair });
                }
                catch (Exception e)
                {
                    observer.OnError(e);
                }

                var unsubscriptionDisposable = Disposable.Create(async () =>
                {
                    // send unsubscription when the observable gets disposed
                    Log.InfoFormat("Sending price unsubscription for currency pair {0}", currencyPair);
                    try
                    {
                        await
                            pricingHubProxy.Invoke(ServiceConstants.Server.UnsubscribePriceStream,
                                new PriceSubscriptionRequestDto { CurrencyPair = currencyPair });
                    }
                    catch (Exception e)
                    {
                        Log.Error(string.Format("An error occured while sending unsubscription request for {0}", currencyPair), e);
                    }
                });

                return new CompositeDisposable {priceSubscription, unsubscriptionDisposable};
            })
            .Publish()
            .RefCount();
        } 
    }
}
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
        private readonly IHubProxy _pricingHubProxy;
        private readonly Lazy<IObservable<PriceDto>> _allPricesLazy;

        private static readonly ILog Log = LogManager.GetLogger(typeof(PricingServiceClient));

        public PricingServiceClient(ISignalRTransport transport)
        {
            _pricingHubProxy = transport.GetProxy(ServiceConstants.Server.PricingHub);

            _allPricesLazy = new Lazy<IObservable<PriceDto>>(CreateAllPrices);
        }

        private IObservable<PriceDto> CreateAllPrices()
        {
            return Observable.Create<PriceDto>(observer => _pricingHubProxy.On<PriceDto>(ServiceConstants.Client.OnNewPrice, observer.OnNext))
                .Publish()
                .RefCount();
        }


        private IObservable<PriceDto> AllPrices
        {
            get
            {
                return _allPricesLazy.Value;
            }
        } 

        public IObservable<PriceDto> GetSpotStream(string currencyPair)
        {
            if (string.IsNullOrEmpty(currencyPair)) throw new ArgumentException("currencyPair");

            return Observable.Create<PriceDto>(async observer =>
            {
                var disposables = new CompositeDisposable();

                // subscribe to price feed first, otherwise there is a race condition 
                disposables.Add(AllPrices.Where(p => p.Symbol == currencyPair).Subscribe(observer));

                // send a subscription request
                try
                {
                    Log.InfoFormat("Sending price subscription for currency pair {0}", currencyPair);
                    await _pricingHubProxy.Invoke(ServiceConstants.Server.SubscribePriceStream, new PriceSubscriptionRequestDto { CurrencyPair = currencyPair });
                }
                catch (Exception e)
                {
                    observer.OnError(e);
                }

                disposables.Add(Disposable.Create(async () =>
                {
                    // send unsubscription when the observable gets disposed
                    Log.InfoFormat("Sending price unsubscription for currency pair {0}", currencyPair);
                    try
                    {
                        await
                            _pricingHubProxy.Invoke(ServiceConstants.Server.UnsubscribePriceStream,
                                new PriceSubscriptionRequestDto { CurrencyPair = currencyPair });
                    }
                    catch (Exception e)
                    {
                        Log.Error(
                            string.Format("An error occured while sending unsubscription request for {0}", currencyPair),
                            e);
                    }
                }));

                return disposables;
            })
            .Publish()
            .RefCount();
        }
    }
}
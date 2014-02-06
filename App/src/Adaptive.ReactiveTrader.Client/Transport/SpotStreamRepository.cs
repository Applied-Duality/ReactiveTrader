using System;
using System.Collections.Concurrent;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Adaptive.ReactiveTrader.Contracts;
using Adaptive.ReactiveTrader.Contracts.Pricing;
using log4net;
using Microsoft.AspNet.SignalR.Client;

namespace Adaptive.ReactiveTrader.Client.Transport
{
    public class SpotStreamRepository : ISpotStreamRepository
    {
        private readonly ITransport _transport;
        private readonly ConcurrentDictionary<string, IObservable<SpotPrice>> _streams = new ConcurrentDictionary<string, IObservable<SpotPrice>>();
        private readonly IObservable<SpotPrice> _allPrices;
        private static readonly ILog Log = LogManager.GetLogger(typeof(SpotStreamRepository));

        public SpotStreamRepository(ITransport transport)
        {
            _transport = transport;

            _allPrices = Observable.Create<SpotPrice>(observer =>
            {
                _transport.PricingHubProxy.On<SpotPrice>(ServiceConstants.Client.OnNewPrice, observer.OnNext);

                return Disposable.Create(() => { });
            })
                .Publish()
                .RefCount();
        }

        public IObservable<SpotPrice> GetSpotStream(string currencyPair)
        {
            if(string.IsNullOrEmpty(currencyPair)) throw new ArgumentException("currencyPair");

            return _streams.GetOrAdd(currencyPair, s => Observable.Create<SpotPrice>(async observer =>
            {
                var disposables = new CompositeDisposable();

                // subscribe to price feed first, otherwise there is a race condition 
                disposables.Add(
                    _allPrices.Where(p => p.Symbol == currencyPair).Subscribe(observer));

                // send a subscription request
                try
                {
                    Log.InfoFormat("Sending price subscription for currency pair {0}", currencyPair);
                    await _transport.PricingHubProxy.Invoke(ServiceConstants.Server.SubscribePriceStream, new PriceSubscriptionRequest {CurrencyPair = currencyPair});
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
                            _transport.PricingHubProxy.Invoke(ServiceConstants.Server.UnsubscribePriceStream,
                                new PriceSubscriptionRequest {CurrencyPair = currencyPair});
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
            .RefCount()
            );
        }
    }
}
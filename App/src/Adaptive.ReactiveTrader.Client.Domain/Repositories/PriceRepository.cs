using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using Adaptive.ReactiveTrader.Client.Domain.Models;
using Adaptive.ReactiveTrader.Client.Domain.ServiceClients;
using Adaptive.ReactiveTrader.Shared.Extensions;

namespace Adaptive.ReactiveTrader.Client.Domain.Repositories
{
    class PriceRepository : IPriceRepository
    {
        private readonly IPricingServiceClient _pricingServiceClient;
        private readonly IPriceFactory _priceFactory;

        public PriceRepository(IPricingServiceClient pricingServiceClient, IPriceFactory priceFactory)
        {
            _pricingServiceClient = pricingServiceClient;
            _priceFactory = priceFactory;
        }

        public IObservable<IPrice> GetPrices(ICurrencyPair currencyPair)
        {
            return Observable.Defer(() => _pricingServiceClient.GetSpotStream(currencyPair.Symbol))
                .Select(p => _priceFactory.Create(p, currencyPair))
                .Catch(Observable.Return(new StalePrice(currencyPair))) // if the stream errors (server disconnected), we push a stale price 
                .Repeat()                                               // and resubscribe
                .DetectStale(TimeSpan.FromSeconds(2),  Scheduler.Default)
                .Select(s => s.IsStale ? new StalePrice(currencyPair) : s.Update)
                .Publish()
                .RefCount();
        }
    }
}
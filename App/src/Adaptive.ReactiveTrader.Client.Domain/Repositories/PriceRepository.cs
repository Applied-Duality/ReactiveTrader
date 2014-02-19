using System;
using System.Reactive.Linq;
using Adaptive.ReactiveTrader.Client.Domain.Models;
using Adaptive.ReactiveTrader.Client.Domain.ServiceClients;

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
                .Catch(Observable.Return(new StalePrice(currencyPair)))
                .Repeat()
                .Publish()
                .RefCount();
        }
    }
}
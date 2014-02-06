using System;
using System.Reactive.Linq;
using Adaptive.ReactiveTrader.Client.Models;
using Adaptive.ReactiveTrader.Client.ServiceClients.Pricing;

namespace Adaptive.ReactiveTrader.Client.Repositories
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
            return _pricingServiceClient.GetSpotStream(currencyPair.Symbol)
                .Select(p => _priceFactory.Create(p))
                .Publish()
                .RefCount();
        }
    }
}
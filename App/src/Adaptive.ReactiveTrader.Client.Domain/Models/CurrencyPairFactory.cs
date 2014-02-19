using Adaptive.ReactiveTrader.Client.Domain.Repositories;
using Adaptive.ReactiveTrader.Shared.ReferenceData;

namespace Adaptive.ReactiveTrader.Client.Domain.Models
{
    class CurrencyPairFactory : ICurrencyPairFactory
    {
        private readonly IPriceRepository _priceRepository;

        public CurrencyPairFactory(IPriceRepository priceRepository)
        {
            _priceRepository = priceRepository;
        }

        public ICurrencyPair Create(CurrencyPairDto currencyPair)
        {
            return new CurrencyPair(
                currencyPair.Symbol, 
                currencyPair.RatePrecision,
                currencyPair.PipsPosition,
                _priceRepository);
        }
    }
}
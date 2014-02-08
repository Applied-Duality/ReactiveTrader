using Adaptive.ReactiveTrader.Client.Repositories;
using Adaptive.ReactiveTrader.Shared.ReferenceData;

namespace Adaptive.ReactiveTrader.Client.Models
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
                currencyPair.BigNumberStartIndex,
                _priceRepository);
        }
    }
}
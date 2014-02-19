using System;
using System.Collections.Concurrent;
using Adaptive.ReactiveTrader.Shared.Pricing;

namespace Adaptive.ReactiveTrader.Server.Pricing
{
    public class PriceLastValueCache : IPriceLastValueCache
    {
        private readonly ConcurrentDictionary<string, PriceDto> _lastValueCache = new ConcurrentDictionary<string, PriceDto>();

        public PriceDto GetLastValue(string currencyPair)
        {
            PriceDto price;
            if (_lastValueCache.TryGetValue(currencyPair, out price))
            {
                return price;
            }
            throw new InvalidOperationException(string.Format("Currency pair {0} has not been initilialized in last value cache", currencyPair));
        }

        public void StoreLastValue(PriceDto price)
        {
            _lastValueCache.AddOrUpdate(price.Symbol, _ => price, (s, p) => p);
        }
    }
}
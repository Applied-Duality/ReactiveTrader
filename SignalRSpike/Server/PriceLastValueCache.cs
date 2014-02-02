using System;
using System.Collections.Concurrent;
using Dto.Pricing;

namespace Server
{
    class PriceLastValueCache : IPriceLastValueCache
    {
        private readonly ConcurrentDictionary<string, SpotPrice> _lastValueCache = new ConcurrentDictionary<string, SpotPrice>();

        public SpotPrice GetLastValue(string currencyPair)
        {
            SpotPrice spotPrice;
            if (_lastValueCache.TryGetValue(currencyPair, out spotPrice))
            {
                return spotPrice;
            }
            throw new InvalidOperationException(string.Format("Currency pair {0} has not been initilialized in last value cache", currencyPair));
        }

        public void StoreLastValue(SpotPrice spotPrice)
        {
            _lastValueCache.AddOrUpdate(spotPrice.Symbol, _ => spotPrice, (s, price) => spotPrice);
        }
    }
}
using System;
using System.Collections.Concurrent;
using Adaptive.ReactiveTrader.Contracts.Pricing;

namespace Adaptive.ReactiveTrader.Server.Pricing
{
    class PriceLastValueCache : IPriceLastValueCache
    {
        private readonly ConcurrentDictionary<string, Price> _lastValueCache = new ConcurrentDictionary<string, Price>();

        public Price GetLastValue(string currencyPair)
        {
            Price price;
            if (_lastValueCache.TryGetValue(currencyPair, out price))
            {
                return price;
            }
            throw new InvalidOperationException(string.Format("Currency pair {0} has not been initilialized in last value cache", currencyPair));
        }

        public void StoreLastValue(Price price)
        {
            _lastValueCache.AddOrUpdate(price.Symbol, _ => price, (s, p) => p);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Adaptive.ReactiveTrader.Contracts;
using Adaptive.ReactiveTrader.Contracts.Pricing;
using Adaptive.ReactiveTrader.Contracts.ReferenceData;
using Adaptive.ReactiveTrader.Server.ReferenceData;

namespace Adaptive.ReactiveTrader.Server.Pricing
{
    class PriceFeedSimulator : IPriceFeed
    {
        private readonly ICurrencyPairRepository _currencyPairRepository;
        private readonly IPricePublisher _pricePublisher;
        private readonly IPriceLastValueCache _priceLastValueCache;
        private Timer _timer;
        private readonly Random _random;
        private readonly List<CurrencyPair> _allCurrencyPairs;
        private const int UpdatePeriodMs = 2000;

        private static readonly Dictionary<string, decimal> SamplePrices = new Dictionary<string, decimal>
        {
            {"EURUSD", 1.34860m},
            {"EURGBP", 0.82040m},
        };

        public PriceFeedSimulator(
            ICurrencyPairRepository currencyPairRepository, 
            IPricePublisher pricePublisher,
            IPriceLastValueCache priceLastValueCache)
        {
            _currencyPairRepository = currencyPairRepository;
            _pricePublisher = pricePublisher;
            _priceLastValueCache = priceLastValueCache;
            _random = new Random(_currencyPairRepository.GetHashCode());
            _allCurrencyPairs = _currencyPairRepository.GetAllCurrencyPairs().ToList();
        }

        public void Start()
        {
            PopulateLastValueCache();

            _timer = new Timer(OnTimerTick, null, UpdatePeriodMs, UpdatePeriodMs);
        }

        private void PopulateLastValueCache()
        {
            foreach (var currencyPair in _currencyPairRepository.GetAllCurrencyPairs())
            {
                if (!SamplePrices.ContainsKey(currencyPair.Symbol))
                {
                    throw new InvalidOperationException(string.Format("Default value for currency pair {0} must be defined in PriceFeedSimulator", currencyPair.Symbol));
                }
                    
                decimal mid = SamplePrices[currencyPair.Symbol];
                
                var initialQuote = new SpotPrice
                {
                    Symbol = currencyPair.Symbol,
                    QuoteId = 0,
                    Mid = mid
                };

                _priceLastValueCache.StoreLastValue(GenerateNextQuote(initialQuote));
            }
        }

        private SpotPrice GenerateNextQuote(SpotPrice previousPrice)
        {
            var currencyPair = _currencyPairRepository.GetCurrencyPair(previousPrice.Symbol);

            var pow = (decimal)Math.Pow(10, currencyPair.RatePrecision);
            var newMid = previousPrice.Mid + _random.Next(-5, 5) / pow;

            return new SpotPrice
            {
                Symbol = previousPrice.Symbol,
                QuoteId = previousPrice.QuoteId + 1,
                ValueDate = DateTime.UtcNow.AddDays(2).Date,
                Mid = newMid,
                Ask = newMid + 5/pow,
                Bid = newMid - 5/pow
            };
        }

        private void OnTimerTick(object state)
        {
            var randomCurrencyPair = _allCurrencyPairs[_random.Next(0, _allCurrencyPairs.Count)];
            var lastPrice = _priceLastValueCache.GetLastValue(randomCurrencyPair.Symbol);

            var newPrice = GenerateNextQuote(lastPrice);
            _priceLastValueCache.StoreLastValue(newPrice);
            _pricePublisher.Publish(newPrice);
        }
    }
}

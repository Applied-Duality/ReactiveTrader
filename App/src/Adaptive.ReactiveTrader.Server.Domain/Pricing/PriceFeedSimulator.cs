using System;
using System.Linq;
using System.Threading;
using Adaptive.ReactiveTrader.Server.ReferenceData;
using Adaptive.ReactiveTrader.Shared.Pricing;

namespace Adaptive.ReactiveTrader.Server.Pricing
{
    public class PriceFeedSimulator : IPriceFeed, IDisposable
    {
        private readonly ICurrencyPairRepository _currencyPairRepository;
        private readonly IPricePublisher _pricePublisher;
        private readonly IPriceLastValueCache _priceLastValueCache;
        private readonly Random _random;
        private Timer _timer;
        private int _updatesPerTick = 1;

        public PriceFeedSimulator(
            ICurrencyPairRepository currencyPairRepository, 
            IPricePublisher pricePublisher,
            IPriceLastValueCache priceLastValueCache)
        {
            _currencyPairRepository = currencyPairRepository;
            _pricePublisher = pricePublisher;
            _priceLastValueCache = priceLastValueCache;
            _random = new Random(_currencyPairRepository.GetHashCode());
        }

        public void Start()
        {
            PopulateLastValueCache();

            SetUpdateFrequency(15);
        }

        public void SetUpdateFrequency(double updatesPerSecond)
        {
            if (_timer != null)
            {
                _timer.Dispose();
            }

            var periodMs = 1000.0/updatesPerSecond;

            if (periodMs < 15.5) // timer resolution is about 15ms, bellow that we start sending multiple update per tick
            {
                _updatesPerTick = (int) (15.5/periodMs);
                periodMs = 15;
            }

            _timer = new Timer(OnTimerTick, null, (int)periodMs, (int)periodMs);
        }

        private void PopulateLastValueCache()
        {
            foreach (var currencyPair in _currencyPairRepository.GetAllCurrencyPairs())
            {
                decimal mid = _currencyPairRepository.GetSampleRate(currencyPair.Symbol);
                
                var initialQuote = new PriceDto
                {
                    Symbol = currencyPair.Symbol,
                    QuoteId = 0,
                    Mid = mid
                };

                _priceLastValueCache.StoreLastValue(GenerateNextQuote(initialQuote));
            }
        }

        private PriceDto GenerateNextQuote(PriceDto previousPrice)
        {
            var currencyPair = _currencyPairRepository.GetCurrencyPair(previousPrice.Symbol);

            var pow = (decimal)Math.Pow(10, currencyPair.RatePrecision);
            var newMid = previousPrice.Mid + _random.Next(-5, 5) / pow;

            return new PriceDto
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
            for (int i = 0; i < _updatesPerTick; i++)
            {
                var activePairs = _currencyPairRepository.GetAllCurrencyPairInfos().Where(cp=>cp.Enabled && !cp.Stale).ToList();
                var randomCurrencyPair = activePairs[_random.Next(0, activePairs.Count)].CurrencyPair;
                var lastPrice = _priceLastValueCache.GetLastValue(randomCurrencyPair.Symbol);

                var newPrice = GenerateNextQuote(lastPrice);
                _priceLastValueCache.StoreLastValue(newPrice);
                _pricePublisher.Publish(newPrice);
            }
        }

        public void Dispose()
        {
            using (_timer)
            {}
        }
    }
}

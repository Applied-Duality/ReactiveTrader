using System;
using System.Linq;
using System.Threading;
using Adaptive.ReactiveTrader.Server.ReferenceData;
using Adaptive.ReactiveTrader.Shared.DTO.Pricing;

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
            foreach (var currencyPairInfo in _currencyPairRepository.GetAllCurrencyPairs())
            {
                var mid = _currencyPairRepository.GetSampleRate(currencyPairInfo.CurrencyPair.Symbol);
                
                var initialQuote = new PriceDto
                {
                    Symbol = currencyPairInfo.CurrencyPair.Symbol,
                    QuoteId = 0,
                    Mid = mid
                };

                _priceLastValueCache.StoreLastValue(currencyPairInfo.GenerateNextQuote(initialQuote));
            }
        }

        private void OnTimerTick(object state)
        {
            var activePairs = _currencyPairRepository.GetAllCurrencyPairInfos().Where(cp => cp.Enabled && !cp.Stale).ToList();

            if (activePairs.Count == 0)
                return;

            for (int i = 0; i < _updatesPerTick; i++)
            {
                var randomCurrencyPairInfo = activePairs[_random.Next(0, activePairs.Count)];
                var lastPrice = _priceLastValueCache.GetLastValue(randomCurrencyPairInfo.CurrencyPair.Symbol);

                var newPrice = randomCurrencyPairInfo.GenerateNextQuote(lastPrice);
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

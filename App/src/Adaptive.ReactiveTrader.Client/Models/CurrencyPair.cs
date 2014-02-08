using System;
using System.Reactive.Linq;
using Adaptive.ReactiveTrader.Client.Repositories;

namespace Adaptive.ReactiveTrader.Client.Models
{
    internal class CurrencyPair : ICurrencyPair
    {
        private readonly Lazy<IObservable<IPrice>> _lazyPrices;

        public CurrencyPair(string symbol, int ratePrecision, int bigNumberStartIndex, IPriceRepository priceRepository)
        {
            Symbol = symbol;
            RatePrecision = ratePrecision;
            BigNumberStartIndex = bigNumberStartIndex;

            _lazyPrices = new Lazy<IObservable<IPrice>>(() => CreatePrices(priceRepository));
        }


        private IObservable<IPrice> CreatePrices(IPriceRepository priceRepository)
        {
            return priceRepository.GetPrices(this)
                .Publish()
                .RefCount();
        }

        public string Symbol { get; private set; }
        public int RatePrecision { get; private set; }
        public int BigNumberStartIndex { get; set; }
        public IObservable<IPrice> Prices { get { return _lazyPrices.Value; } }
    }
}
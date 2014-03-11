using System;
using System.Reactive.Linq;
using Adaptive.ReactiveTrader.Client.Domain.Models.Pricing;
using Adaptive.ReactiveTrader.Client.Domain.Repositories;

namespace Adaptive.ReactiveTrader.Client.Domain.Models.ReferenceData
{
    internal class CurrencyPair : ICurrencyPair
    {
        private readonly Lazy<IObservable<IPrice>> _lazyPrices;

        public CurrencyPair(string symbol, int ratePrecision, int pipsPosition, IPriceRepository priceRepository)
        {
            Symbol = symbol;
            RatePrecision = ratePrecision;
            PipsPosition = pipsPosition;
            BaseCurrency = symbol.Substring(0, 3);
            CounterCurrency = symbol.Substring(3, 3);

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
        public int PipsPosition { get; private set; }
        public string BaseCurrency { get; private set; }
        public string CounterCurrency { get; private set; }
        public IObservable<IPrice> Prices { get { return _lazyPrices.Value; } }
    }
}
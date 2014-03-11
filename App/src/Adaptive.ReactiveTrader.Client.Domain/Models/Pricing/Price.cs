using System;
using System.Diagnostics;
using Adaptive.ReactiveTrader.Client.Domain.Models.ReferenceData;

namespace Adaptive.ReactiveTrader.Client.Domain.Models.Pricing
{
    internal class Price : IPrice
    {
        private readonly Stopwatch _priceCreationTime;

        public Price(ExecutablePrice bid, ExecutablePrice ask, decimal mid, long quoteId, DateTime valueDate, ICurrencyPair currencyPair)
        {
            Bid = bid;
            Ask = ask;
            QuoteId = quoteId;
            ValueDate = valueDate;
            CurrencyPair = currencyPair;
            Mid = mid;

            bid.Parent = this;
            ask.Parent = this;

            Spread = (ask.Rate - bid.Rate)* (long)Math.Pow(10, currencyPair.PipsPosition);
            _priceCreationTime = Stopwatch.StartNew();
        }

        public IExecutablePrice Bid { get; private set; }
        public IExecutablePrice Ask { get; private set; }
        public decimal Mid { get; private set; }
        public ICurrencyPair CurrencyPair { get; private set; }
        public long QuoteId { get; private set; }
        public DateTime ValueDate { get; private set; }
        public decimal Spread { get; private set; }
        public bool IsStale { get { return false; } }
        public TimeSpan ElpasedTimeSinceCreated { get { return _priceCreationTime.Elapsed; } }

        public override string ToString()
        {
            return string.Format("[{0}] Bid:{1} / Ask:{2}", CurrencyPair.Symbol, Bid.Rate, Ask.Rate);
        }
    }
}
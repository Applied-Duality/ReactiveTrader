using System;

namespace Adaptive.ReactiveTrader.Client.Domain.Models
{
    class Price : IPrice
    {
        public Price(ExecutablePrice bid, ExecutablePrice ask, long quoteId, DateTime valueDate, ICurrencyPair currencyPair)
        {
            Bid = bid;
            Ask = ask;
            QuoteId = quoteId;
            ValueDate = valueDate;
            CurrencyPair = currencyPair;

            bid.Parent = this;
            ask.Parent = this;

            Spread = (ask.Rate - bid.Rate)* (long)Math.Pow(10, currencyPair.PipsPosition);
        }

        public IExecutablePrice Bid { get; private set; }
        public IExecutablePrice Ask { get; private set; }
        public ICurrencyPair CurrencyPair { get; private set; }
        public long QuoteId { get; private set; }
        public DateTime ValueDate { get; private set; }
        public decimal Spread { get; private set; }
        public bool IsStale { get { return false; } }
    }
}
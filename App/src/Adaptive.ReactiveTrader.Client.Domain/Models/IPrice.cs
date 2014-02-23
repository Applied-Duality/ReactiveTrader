using System;

namespace Adaptive.ReactiveTrader.Client.Domain.Models
{
    public interface IPrice
    {
        IExecutablePrice Bid { get; }
        IExecutablePrice Ask { get; }
        ICurrencyPair CurrencyPair { get; }
        long QuoteId { get; }
        DateTime ValueDate { get; }
        decimal Spread { get; }
        bool IsStale { get; }
        TimeSpan ElpasedTimeSinceCreated { get; }
    }
}

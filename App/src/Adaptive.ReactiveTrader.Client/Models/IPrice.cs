using System;

namespace Adaptive.ReactiveTrader.Client.Models
{
    public interface IPrice
    {
        IExecutablePrice Bid { get; }
        IExecutablePrice Ask { get; }
        ICurrencyPair CurrencyPair { get; }
        long QuoteId { get; }
        DateTime ValueDate { get; }
    }
}

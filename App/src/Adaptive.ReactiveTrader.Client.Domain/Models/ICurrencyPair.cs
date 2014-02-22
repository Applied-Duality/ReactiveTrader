using System;

namespace Adaptive.ReactiveTrader.Client.Domain.Models
{
    public interface ICurrencyPair
    {
        string Symbol { get; }
        IObservable<IPrice> Prices { get; }
        int RatePrecision { get; }
        int PipsPosition { get; }
        string BaseCurrency { get; }
        string CounterCurrency { get; }
    }
}

using System;

namespace Adaptive.ReactiveTrader.Client.Models
{
    public interface ICurrencyPair
    {
        string Symbol { get; }
        IObservable<IPrice> Prices { get; } 
    }
}

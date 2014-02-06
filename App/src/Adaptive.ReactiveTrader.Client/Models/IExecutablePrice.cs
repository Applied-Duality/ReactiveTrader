using System;

namespace Adaptive.ReactiveTrader.Client.Models
{
    public interface IExecutablePrice
    {
        IOneWayPrice Price { get; }
        IObservable<ITrade> Execute();
    }
}

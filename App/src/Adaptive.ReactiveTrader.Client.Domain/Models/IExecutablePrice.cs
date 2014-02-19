using System;

namespace Adaptive.ReactiveTrader.Client.Domain.Models
{
    public interface IExecutablePrice
    {
        IObservable<ITrade> Execute(long notional);
        Direction Direction { get; }
        IPrice Parent { get; }
        decimal Rate { get; }
    }
}

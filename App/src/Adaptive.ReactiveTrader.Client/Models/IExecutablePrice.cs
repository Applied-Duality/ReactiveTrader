using System;

namespace Adaptive.ReactiveTrader.Client.Models
{
    public interface IExecutablePrice
    {
        IObservable<ITrade> Execute(long notional);
        string BigNumbers { get; }
        string Pips { get; }
        string PipDecimals { get; }
        Direction Direction { get; }
        IPrice Parent { get; }
        decimal Rate { get; }
    }
}

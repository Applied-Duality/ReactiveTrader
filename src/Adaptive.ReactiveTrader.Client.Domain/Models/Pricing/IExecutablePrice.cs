using System;
using Adaptive.ReactiveTrader.Client.Domain.Models.Execution;

namespace Adaptive.ReactiveTrader.Client.Domain.Models.Pricing
{
    public interface IExecutablePrice
    {
        IObservable<ITrade> ExecuteRequest(long notional, string dealtCurrency);
        Direction Direction { get; }
        IPrice Parent { get; }
        decimal Rate { get; }
    }
}

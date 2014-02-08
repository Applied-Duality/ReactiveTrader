using System;
using Adaptive.ReactiveTrader.Client.Models;

namespace Adaptive.ReactiveTrader.Client.Repositories
{
    public interface IExecutionRepository
    {
        IObservable<ITrade> Execute(IExecutablePrice executablePrice, long notional);
    }
}
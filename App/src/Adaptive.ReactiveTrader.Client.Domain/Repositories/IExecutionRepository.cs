using System;
using Adaptive.ReactiveTrader.Client.Domain.Models;

namespace Adaptive.ReactiveTrader.Client.Domain.Repositories
{
    interface IExecutionRepository
    {
        IObservable<ITrade> Execute(IExecutablePrice executablePrice, long notional);
    }
}
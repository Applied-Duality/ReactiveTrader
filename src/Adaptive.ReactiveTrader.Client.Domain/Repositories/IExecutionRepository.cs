using System;
using Adaptive.ReactiveTrader.Client.Domain.Models.Execution;
using Adaptive.ReactiveTrader.Client.Domain.Models.Pricing;

namespace Adaptive.ReactiveTrader.Client.Domain.Repositories
{
    interface IExecutionRepository
    {
        IObservable<ITrade> ExecuteRequest(IExecutablePrice executablePrice, long notional, string dealtCurrency);
    }
}
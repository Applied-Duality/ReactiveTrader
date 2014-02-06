using System;
using Adaptive.ReactiveTrader.Contracts.Execution;

namespace Adaptive.ReactiveTrader.Client.ServiceClients.Execution
{
    public interface IExecutionServiceClient
    {
        IObservable<SpotTrade> Execute(SpotTradeRequest spotTradeRequest);
    }
}
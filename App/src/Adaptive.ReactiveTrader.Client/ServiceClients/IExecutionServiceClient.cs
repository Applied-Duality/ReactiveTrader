using System;
using Adaptive.ReactiveTrader.Shared.Execution;

namespace Adaptive.ReactiveTrader.Client.ServiceClients
{
    public interface IExecutionServiceClient
    {
        IObservable<TradeDto> Execute(TradeRequestDto tradeRequest);
    }
}
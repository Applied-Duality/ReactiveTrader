using System;
using Adaptive.ReactiveTrader.Shared.Execution;

namespace Adaptive.ReactiveTrader.Client.Domain.ServiceClients
{
    interface IExecutionServiceClient
    {
        IObservable<TradeDto> Execute(TradeRequestDto tradeRequest);
    }
}
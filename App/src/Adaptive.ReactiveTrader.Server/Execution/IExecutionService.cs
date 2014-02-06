using Adaptive.ReactiveTrader.Contracts;
using Adaptive.ReactiveTrader.Contracts.Execution;

namespace Adaptive.ReactiveTrader.Server.Execution
{
    public interface IExecutionService
    {
        SpotTrade Execute(SpotTradeRequest tradeRequest, string username);
    }
}
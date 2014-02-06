using Adaptive.ReactiveTrader.Contracts;

namespace Adaptive.ReactiveTrader.Server.Execution
{
    public interface IExecutionService
    {
        SpotTrade Execute(SpotTradeRequest tradeRequest, string username);
    }
}
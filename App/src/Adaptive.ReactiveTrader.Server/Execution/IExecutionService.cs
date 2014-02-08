using Adaptive.ReactiveTrader.Shared.Execution;

namespace Adaptive.ReactiveTrader.Server.Execution
{
    public interface IExecutionService
    {
        TradeDto Execute(TradeRequestDto tradeRequest, string username);
    }
}
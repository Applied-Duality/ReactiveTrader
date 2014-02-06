using Adaptive.ReactiveTrader.Contracts;

namespace Adaptive.ReactiveTrader.Server
{
    public interface IExecutionService
    {
        SpotTrade Execute(SpotTradeRequest tradeRequest, string username);
    }
}
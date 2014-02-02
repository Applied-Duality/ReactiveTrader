using Dto;

namespace Server
{
    public interface IExecutionService
    {
        SpotTrade Execute(SpotTradeRequest tradeRequest, string username);
    }
}
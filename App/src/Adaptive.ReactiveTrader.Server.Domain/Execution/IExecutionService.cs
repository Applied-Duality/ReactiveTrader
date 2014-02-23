using System.Threading.Tasks;
using Adaptive.ReactiveTrader.Shared.Execution;

namespace Adaptive.ReactiveTrader.Server.Execution
{
    public interface IExecutionService
    {
        Task<TradeDto> Execute(TradeRequestDto tradeRequest, string username);
    }
}
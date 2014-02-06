using System.Threading.Tasks;
using Adaptive.ReactiveTrader.Contracts;

namespace Adaptive.ReactiveTrader.Client
{
    public interface IExecutionServiceClient
    {
        Task<SpotTrade> Execute(SpotTradeRequest spotTradeRequest);
    }
}
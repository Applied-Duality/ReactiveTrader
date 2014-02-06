using System.Threading.Tasks;
using Adaptive.ReactiveTrader.Contracts;
using Adaptive.ReactiveTrader.Contracts.Execution;

namespace Adaptive.ReactiveTrader.Client.Transport
{
    public interface IExecutionServiceClient
    {
        Task<SpotTrade> Execute(SpotTradeRequest spotTradeRequest);
    }
}
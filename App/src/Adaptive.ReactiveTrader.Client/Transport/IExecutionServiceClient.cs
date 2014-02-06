using System.Threading.Tasks;
using Adaptive.ReactiveTrader.Contracts;

namespace Adaptive.ReactiveTrader.Client.Transport
{
    public interface IExecutionServiceClient
    {
        Task<SpotTrade> Execute(SpotTradeRequest spotTradeRequest);
    }
}
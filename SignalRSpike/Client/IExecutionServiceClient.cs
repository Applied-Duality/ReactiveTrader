using System.Threading.Tasks;
using Dto;

namespace Client
{
    public interface IExecutionServiceClient
    {
        Task<SpotTrade> Execute(SpotTradeRequest spotTradeRequest);
    }
}
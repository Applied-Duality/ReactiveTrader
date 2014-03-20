using System.Threading.Tasks;
using Adaptive.ReactiveTrader.Shared.DTO.Execution;

namespace Adaptive.ReactiveTrader.Server.Blotter
{
    public interface IBlotterPublisher
    {
        Task Publish(TradeDto trade);
    }
}
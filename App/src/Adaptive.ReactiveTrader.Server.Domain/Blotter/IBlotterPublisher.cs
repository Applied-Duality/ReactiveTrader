using System.Threading.Tasks;
using Adaptive.ReactiveTrader.Shared.Execution;

namespace Adaptive.ReactiveTrader.Server.Blotter
{
    public interface IBlotterPublisher
    {
        Task Publish(TradeDto trade);
    }
}
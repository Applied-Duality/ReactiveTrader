using System.Threading.Tasks;
using Adaptive.ReactiveTrader.Shared.Execution;

namespace Adaptive.ReactiveTrader.Server.Blotter
{
    internal interface IBlotterPublisher
    {
        Task Publish(TradeDto trade);
    }
}
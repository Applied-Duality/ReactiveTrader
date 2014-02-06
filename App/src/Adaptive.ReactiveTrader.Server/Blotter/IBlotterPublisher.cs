using System.Threading.Tasks;
using Adaptive.ReactiveTrader.Contracts;
using Adaptive.ReactiveTrader.Contracts.Execution;

namespace Adaptive.ReactiveTrader.Server.Blotter
{
    internal interface IBlotterPublisher
    {
        Task Publish(Trade trade);
    }
}
using System.Threading.Tasks;
using Adaptive.ReactiveTrader.Contracts;

namespace Adaptive.ReactiveTrader.Server.Blotter
{
    internal interface IBlotterPublisher
    {
        Task Publish(SpotTrade trade);
    }
}
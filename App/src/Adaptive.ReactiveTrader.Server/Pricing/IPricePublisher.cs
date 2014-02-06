using System.Threading.Tasks;
using Adaptive.ReactiveTrader.Contracts.Pricing;

namespace Adaptive.ReactiveTrader.Server.Pricing
{
    internal interface IPricePublisher
    {
        Task Publish(Price price);
    }
}
using System.Threading.Tasks;
using Adaptive.ReactiveTrader.Shared.Pricing;

namespace Adaptive.ReactiveTrader.Server.Pricing
{
    internal interface IPricePublisher
    {
        Task Publish(PriceDto price);
    }
}
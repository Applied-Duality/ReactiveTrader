using System.Threading.Tasks;
using Adaptive.ReactiveTrader.Shared.Pricing;

namespace Adaptive.ReactiveTrader.Server.Pricing
{
    public interface IPricePublisher
    {
        Task Publish(PriceDto price);
        long TotalPricesPublished { get; }
    }
}
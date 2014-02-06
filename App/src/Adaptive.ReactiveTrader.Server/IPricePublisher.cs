using System.Threading.Tasks;
using Adaptive.ReactiveTrader.Contracts.Pricing;

namespace Adaptive.ReactiveTrader.Server
{
    internal interface IPricePublisher
    {
        Task Publish(SpotPrice price);
    }
}
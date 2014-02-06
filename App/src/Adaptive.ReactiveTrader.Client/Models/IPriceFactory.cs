using Adaptive.ReactiveTrader.Contracts.Pricing;

namespace Adaptive.ReactiveTrader.Client.Models
{
    internal interface IPriceFactory
    {
        IPrice Create(Price price);
    }
}
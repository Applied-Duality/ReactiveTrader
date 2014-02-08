using Adaptive.ReactiveTrader.Shared.Pricing;

namespace Adaptive.ReactiveTrader.Client.Models
{
    internal interface IPriceFactory
    {
        IPrice Create(PriceDto price, ICurrencyPair currencyPair);
    }
}
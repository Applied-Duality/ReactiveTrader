using Adaptive.ReactiveTrader.Shared.Pricing;

namespace Adaptive.ReactiveTrader.Client.Domain.Models
{
    interface IPriceFactory
    {
        IPrice Create(PriceDto price, ICurrencyPair currencyPair);
    }
}
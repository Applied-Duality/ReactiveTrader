using Adaptive.ReactiveTrader.Shared.ReferenceData;

namespace Adaptive.ReactiveTrader.Client.Domain.Models
{
    interface ICurrencyPairFactory
    {
        ICurrencyPair Create(CurrencyPairDto currencyPair);
    }
}
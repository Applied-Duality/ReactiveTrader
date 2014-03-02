using Adaptive.ReactiveTrader.Shared.ReferenceData;

namespace Adaptive.ReactiveTrader.Client.Domain.Models
{
    interface ICurrencyPairUpdateFactory
    {
        ICurrencyPairUpdate Create(CurrencyPairUpdateDto currencyPairUpdate);
    }
}
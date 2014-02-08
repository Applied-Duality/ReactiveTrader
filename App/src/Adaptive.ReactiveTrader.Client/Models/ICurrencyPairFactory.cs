using Adaptive.ReactiveTrader.Shared.ReferenceData;

namespace Adaptive.ReactiveTrader.Client.Models
{
    public interface ICurrencyPairFactory
    {
        ICurrencyPair Create(CurrencyPairDto currencyPair);
    }
}
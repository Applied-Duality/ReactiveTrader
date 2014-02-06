using Adaptive.ReactiveTrader.Contracts.ReferenceData;

namespace Adaptive.ReactiveTrader.Client.Models
{
    public interface ICurrencyPairFactory
    {
        ICurrencyPair Create(CurrencyPair currencyPair);
    }
}
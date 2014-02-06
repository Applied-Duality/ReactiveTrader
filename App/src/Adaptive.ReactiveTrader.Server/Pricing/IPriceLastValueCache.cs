using Adaptive.ReactiveTrader.Contracts.Pricing;

namespace Adaptive.ReactiveTrader.Server.Pricing
{
    public interface IPriceLastValueCache
    {
        Price GetLastValue(string currencyPair);
        void StoreLastValue(Price price);
    }
}
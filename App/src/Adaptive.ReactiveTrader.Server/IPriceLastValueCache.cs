using Adaptive.ReactiveTrader.Contracts.Pricing;

namespace Adaptive.ReactiveTrader.Server
{
    public interface IPriceLastValueCache
    {
        SpotPrice GetLastValue(string currencyPair);
        void StoreLastValue(SpotPrice spotPrice);
    }
}
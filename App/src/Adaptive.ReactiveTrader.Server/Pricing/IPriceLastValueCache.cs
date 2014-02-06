using Adaptive.ReactiveTrader.Contracts.Pricing;

namespace Adaptive.ReactiveTrader.Server.Pricing
{
    public interface IPriceLastValueCache
    {
        SpotPrice GetLastValue(string currencyPair);
        void StoreLastValue(SpotPrice spotPrice);
    }
}
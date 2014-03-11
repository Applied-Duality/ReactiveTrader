using Adaptive.ReactiveTrader.Shared.DTO.Pricing;

namespace Adaptive.ReactiveTrader.Server.Pricing
{
    public interface IPriceLastValueCache
    {
        PriceDto GetLastValue(string currencyPair);
        void StoreLastValue(PriceDto price);
    }
}
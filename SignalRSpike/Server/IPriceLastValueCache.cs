using Dto.Pricing;

namespace Server
{
    public interface IPriceLastValueCache
    {
        SpotPrice GetLastValue(string currencyPair);
        void StoreLastValue(SpotPrice spotPrice);
    }
}
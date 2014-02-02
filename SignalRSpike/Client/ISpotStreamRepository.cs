using System;
using Dto.Pricing;

namespace Client
{
    public interface ISpotStreamRepository
    {
        IObservable<SpotPrice> GetSpotStream(string currencyPair);
    }
}
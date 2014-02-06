using System;
using Adaptive.ReactiveTrader.Contracts.Pricing;

namespace Adaptive.ReactiveTrader.Client
{
    public interface ISpotStreamRepository
    {
        IObservable<SpotPrice> GetSpotStream(string currencyPair);
    }
}
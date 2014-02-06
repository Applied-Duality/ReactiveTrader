using System;
using Adaptive.ReactiveTrader.Contracts.Pricing;

namespace Adaptive.ReactiveTrader.Client.Transport
{
    public interface ISpotStreamRepository
    {
        IObservable<SpotPrice> GetSpotStream(string currencyPair);
    }
}
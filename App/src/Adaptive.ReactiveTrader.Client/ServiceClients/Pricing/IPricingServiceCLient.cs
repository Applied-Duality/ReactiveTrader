using System;
using Adaptive.ReactiveTrader.Shared.Pricing;

namespace Adaptive.ReactiveTrader.Client.ServiceClients.Pricing
{
    public interface IPricingServiceClient
    {
        IObservable<PriceDto> GetSpotStream(string currencyPair);
    }
}
using System;
using Adaptive.ReactiveTrader.Shared.DTO.Pricing;

namespace Adaptive.ReactiveTrader.Client.Domain.ServiceClients
{
    internal interface IPricingServiceClient
    {
        IObservable<PriceDto> GetSpotStream(string currencyPair);
    }
}
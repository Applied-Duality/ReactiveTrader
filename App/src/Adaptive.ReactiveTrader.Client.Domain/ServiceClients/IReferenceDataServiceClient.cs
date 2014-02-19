using System;
using System.Collections.Generic;
using Adaptive.ReactiveTrader.Shared.ReferenceData;

namespace Adaptive.ReactiveTrader.Client.Domain.ServiceClients
{
    interface IReferenceDataServiceClient
    {
        IObservable<IEnumerable<CurrencyPairUpdateDto>> GetCurrencyPairUpdates();
    }
}
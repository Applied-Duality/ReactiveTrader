using System;
using System.Collections.Generic;
using Adaptive.ReactiveTrader.Shared.ReferenceData;

namespace Adaptive.ReactiveTrader.Client.ServiceClients.ReferenceData
{
    public interface IReferenceDataServiceClient
    {
        IObservable<IEnumerable<CurrencyPairUpdateDto>> GetCurrencyPairUpdates();
    }
}
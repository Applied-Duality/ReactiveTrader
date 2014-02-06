using System;
using System.Collections.Generic;
using Adaptive.ReactiveTrader.Contracts.ReferenceData;

namespace Adaptive.ReactiveTrader.Client.ServiceClients.ReferenceData
{
    public interface IReferenceDataServiceClient
    {
        IObservable<IEnumerable<CurrencyPairUpdate>> GetCurrencyPairUpdates();
    }
}
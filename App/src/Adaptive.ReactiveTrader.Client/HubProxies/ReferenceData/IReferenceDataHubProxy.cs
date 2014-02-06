using System;
using System.Collections.Generic;
using Adaptive.ReactiveTrader.Contracts;

namespace Adaptive.ReactiveTrader.Client.HubProxies.ReferenceData
{
    public interface IReferenceDataHubProxy
    {
        IObservable<IEnumerable<CurrencyPairUpdate>> GetCurrencyPairUpdates();
    }

    class ReferenceDataHubProxy : IReferenceDataHubProxy
    {
        public IObservable<IEnumerable<CurrencyPairUpdate>> GetCurrencyPairUpdates()
        {
            
        }
    }
}
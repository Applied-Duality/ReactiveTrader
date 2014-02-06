using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using Adaptive.ReactiveTrader.Client.Transport;
using Adaptive.ReactiveTrader.Contracts;
using Adaptive.ReactiveTrader.Contracts.Extensions;
using Adaptive.ReactiveTrader.Contracts.ReferenceData;
using Microsoft.AspNet.SignalR.Client;

namespace Adaptive.ReactiveTrader.Client.ServiceClients.ReferenceData
{
    class ReferenceDataServiceClient : IReferenceDataServiceClient
    {
        private readonly IHubProxy _referenceDataProxy;

        public ReferenceDataServiceClient(ISignalRTransport transport)
        {
            _referenceDataProxy = transport.GetProxy(ServiceConstants.Server.ReferenceDataHub);
        }

        public IObservable<IEnumerable<CurrencyPairUpdate>> GetCurrencyPairUpdates()
        {
            return
                Observable.FromAsync(() => _referenceDataProxy.Invoke<IEnumerable<CurrencyPairUpdate>>(ServiceConstants.Server.GetCurrencyPairs))
                    .CacheFirstResult();
        }
    }
}
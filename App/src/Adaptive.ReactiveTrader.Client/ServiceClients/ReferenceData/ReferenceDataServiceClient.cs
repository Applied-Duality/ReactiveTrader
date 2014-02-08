using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using Adaptive.ReactiveTrader.Client.Transport;
using Adaptive.ReactiveTrader.Shared;
using Adaptive.ReactiveTrader.Shared.Extensions;
using Adaptive.ReactiveTrader.Shared.ReferenceData;
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

        public IObservable<IEnumerable<CurrencyPairUpdateDto>> GetCurrencyPairUpdates()
        {
            return
                Observable.FromAsync(() => _referenceDataProxy.Invoke<IEnumerable<CurrencyPairUpdateDto>>(ServiceConstants.Server.GetCurrencyPairs))
                    .CacheFirstResult();
        }
    }
}
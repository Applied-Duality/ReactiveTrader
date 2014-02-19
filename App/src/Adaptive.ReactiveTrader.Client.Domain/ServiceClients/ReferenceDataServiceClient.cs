using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using Adaptive.ReactiveTrader.Client.Domain.Transport;
using Adaptive.ReactiveTrader.Shared;
using Adaptive.ReactiveTrader.Shared.Extensions;
using Adaptive.ReactiveTrader.Shared.ReferenceData;
using Microsoft.AspNet.SignalR.Client;

namespace Adaptive.ReactiveTrader.Client.Domain.ServiceClients
{
    class ReferenceDataServiceClient : ServiceClient, IReferenceDataServiceClient
    {
        public ReferenceDataServiceClient(IConnectionProvider connectionProvider) : base(connectionProvider)
        {
        }

        public IObservable<IEnumerable<CurrencyPairUpdateDto>> GetCurrencyPairUpdates()
        {
            return RequestUponConnection(connection => GetCurrencyPairUpdatesForConnection(connection.ReferenceDataHubProxy), TimeSpan.FromSeconds(5));
        }

        private IObservable<IEnumerable<CurrencyPairUpdateDto>> GetCurrencyPairUpdatesForConnection(IHubProxy referenceDataHubProxy)
        {
            return
                Observable.FromAsync(() => referenceDataHubProxy.Invoke<IEnumerable<CurrencyPairUpdateDto>>(ServiceConstants.Server.GetCurrencyPairs))
                    .CacheFirstResult();
        }
    }
}
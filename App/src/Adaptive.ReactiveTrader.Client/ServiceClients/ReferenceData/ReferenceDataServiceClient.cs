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
        private readonly IConnectionProvider _connectionProvider;

        public ReferenceDataServiceClient(IConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        public IObservable<IEnumerable<CurrencyPairUpdateDto>> GetCurrencyPairUpdates()
        {
            return from connection in _connectionProvider.GetActiveConnection().Take(1) // TODO handle new connection properly
                from currencyPairs in GetCurrencyPairUpdatesForConnection(connection.ReferenceDataHubProxy)
                select currencyPairs;
        }

        public IObservable<IEnumerable<CurrencyPairUpdateDto>> GetCurrencyPairUpdatesForConnection(IHubProxy referenceDataHubProxy)
        {
            return
                Observable.FromAsync(() => referenceDataHubProxy.Invoke<IEnumerable<CurrencyPairUpdateDto>>(ServiceConstants.Server.GetCurrencyPairs))
                    .CacheFirstResult();
        }
    }
}
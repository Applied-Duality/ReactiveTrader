using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using Adaptive.ReactiveTrader.Client.Domain.Models;
using Adaptive.ReactiveTrader.Client.Domain.ServiceClients;
using Adaptive.ReactiveTrader.Shared;

namespace Adaptive.ReactiveTrader.Client.Domain.Repositories
{
    class ReferenceDataRepository : IReferenceDataRepository
    {
        private readonly IReferenceDataServiceClient _referenceDataServiceClient;
        private readonly ICurrencyPairFactory _currencyPairFactory;

        public ReferenceDataRepository(IReferenceDataServiceClient referenceDataServiceClient, ICurrencyPairFactory currencyPairFactory)
        {
            _referenceDataServiceClient = referenceDataServiceClient;
            _currencyPairFactory = currencyPairFactory;
        }

        public IObservable<IEnumerable<ICurrencyPair>> GetCurrencyPairs()
        {
            return _referenceDataServiceClient.GetCurrencyPairUpdates()
                .Select(updates => updates.Where(update => update.UpdateType == UpdateTypeDto.Added))
                .Where(updates => updates.Any())
                .Select(updates => updates.Select(update => _currencyPairFactory.Create(update.CurrencyPair)))
                .Publish()
                .RefCount();
            }
    }
}

using System;
using System.Collections.Generic;
using Adaptive.ReactiveTrader.Client.Models;

namespace Adaptive.ReactiveTrader.Client.Repositories
{
    public class ReferenceDataRepository : IReferenceDataRepository
    {
        public IObservable<IEnumerable<ICurrencyPair>> GetCurrencyPairs()
        {
            throw new NotImplementedException();
        }
    }
}

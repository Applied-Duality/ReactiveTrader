using System;
using System.Collections.Generic;
using Adaptive.ReactiveTrader.Client.Domain.Models;

namespace Adaptive.ReactiveTrader.Client.Domain.Repositories
{
    public interface IReferenceDataRepository
    {
        IObservable<IEnumerable<ICurrencyPair>> GetCurrencyPairs();
    }
}

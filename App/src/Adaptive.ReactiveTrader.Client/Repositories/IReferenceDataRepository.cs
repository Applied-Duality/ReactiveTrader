using System;
using System.Collections.Generic;
using Adaptive.ReactiveTrader.Client.Models;

namespace Adaptive.ReactiveTrader.Client.Repositories
{
    public interface IReferenceDataRepository
    {
        IObservable<IEnumerable<ICurrencyPair>> GetCurrencyPairs();
    }
}

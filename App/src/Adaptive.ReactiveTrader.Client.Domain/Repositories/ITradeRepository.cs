using System;
using System.Collections.Generic;
using Adaptive.ReactiveTrader.Client.Domain.Models;

namespace Adaptive.ReactiveTrader.Client.Domain.Repositories
{
    public interface ITradeRepository
    {
        IObservable<IEnumerable<ITrade>> GetTrades();
    }
}
using System;
using System.Collections.Generic;
using Adaptive.ReactiveTrader.Client.Models;

namespace Adaptive.ReactiveTrader.Client.Repositories
{
    public interface ITradeRepository
    {
        IObservable<IEnumerable<ITrade>> GetTrades();
    }
}
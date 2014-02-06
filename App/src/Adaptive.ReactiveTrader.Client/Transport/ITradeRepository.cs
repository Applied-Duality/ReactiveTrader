using System;
using Adaptive.ReactiveTrader.Contracts;
using Adaptive.ReactiveTrader.Contracts.Execution;

namespace Adaptive.ReactiveTrader.Client.Transport
{
    public interface ITradeRepository
    {
        IObservable<SpotTrade> GetAllTrades();
    }
}
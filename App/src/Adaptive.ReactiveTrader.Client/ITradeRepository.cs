using System;
using Adaptive.ReactiveTrader.Contracts;

namespace Adaptive.ReactiveTrader.Client
{
    public interface ITradeRepository
    {
        IObservable<SpotTrade> GetAllTrades();
    }
}
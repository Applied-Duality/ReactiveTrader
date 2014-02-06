using System;
using Adaptive.ReactiveTrader.Contracts;

namespace Adaptive.ReactiveTrader.Client.Transport
{
    public interface ITradeRepository
    {
        IObservable<SpotTrade> GetAllTrades();
    }
}
using System.Collections.Generic;
using Adaptive.ReactiveTrader.Contracts;

namespace Adaptive.ReactiveTrader.Server
{
    public interface ITradeRepository
    {
        void StoreTrade(SpotTrade trade);
        IList<SpotTrade> GetAllTrades();
    }
}
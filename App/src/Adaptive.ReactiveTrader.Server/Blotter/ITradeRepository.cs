using System.Collections.Generic;
using Adaptive.ReactiveTrader.Contracts;
using Adaptive.ReactiveTrader.Contracts.Execution;

namespace Adaptive.ReactiveTrader.Server.Blotter
{
    public interface ITradeRepository
    {
        void StoreTrade(SpotTrade trade);
        IList<SpotTrade> GetAllTrades();
    }
}
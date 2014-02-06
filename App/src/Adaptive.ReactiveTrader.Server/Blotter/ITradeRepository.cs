using System.Collections.Generic;
using Adaptive.ReactiveTrader.Contracts;
using Adaptive.ReactiveTrader.Contracts.Execution;

namespace Adaptive.ReactiveTrader.Server.Blotter
{
    public interface ITradeRepository
    {
        void StoreTrade(Trade trade);
        IList<Trade> GetAllTrades();
    }
}
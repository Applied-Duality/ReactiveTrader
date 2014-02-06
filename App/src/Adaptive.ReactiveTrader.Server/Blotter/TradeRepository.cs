using System.Collections.Generic;
using System.Linq;
using Adaptive.ReactiveTrader.Contracts;
using Adaptive.ReactiveTrader.Contracts.Execution;

namespace Adaptive.ReactiveTrader.Server.Blotter
{
    public class TradeRepository : ITradeRepository
    {
        private readonly List<Trade> _allTrades = new List<Trade>();

        public void StoreTrade(Trade trade)
        {
            lock (_allTrades)
            {
                _allTrades.Add(trade);
            }
        }

        public IList<Trade> GetAllTrades()
        {
            IList<Trade> trades;

            lock (_allTrades)
            {
                trades = _allTrades.ToList();
            }

            return trades;
        } 
    }
}
using System.Collections.Generic;
using System.Linq;
using Dto;

namespace Server
{
    public class TradeRepository : ITradeRepository
    {
        private readonly List<SpotTrade> _allTrades = new List<SpotTrade>();

        public void StoreTrade(SpotTrade trade)
        {
            lock (_allTrades)
            {
                _allTrades.Add(trade);
            }
        }

        public IList<SpotTrade> GetAllTrades()
        {
            IList<SpotTrade> trades;

            lock (_allTrades)
            {
                trades = _allTrades.ToList();
            }

            return trades;
        } 
    }
}
using System.Collections.Generic;
using Dto;

namespace Server
{
    public interface ITradeRepository
    {
        void StoreTrade(SpotTrade trade);
        IList<SpotTrade> GetAllTrades();
    }
}
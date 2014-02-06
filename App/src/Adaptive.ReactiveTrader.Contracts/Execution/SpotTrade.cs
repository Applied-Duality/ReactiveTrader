using System;

namespace Adaptive.ReactiveTrader.Contracts.Execution
{
    public class SpotTrade
    {
         public long TradeId { get; set; }
         public string TraderName { get; set; }
         public string CurrencyPair { get; set; }
         public long Notional { get; set; }
         public Direction Direction { get; set; }
         public decimal SpotPrice { get; set; }
         public DateTime TradeDate { get; set; }
         public DateTime ValueDate { get; set; }
         public TradeStatus Status { get; set; }

        public override string ToString()
        {
            return string.Format("TradeId: {0}, TraderName: {1}, CurrencyPair: {2}, Notional: {3}, Direction: {4}, SpotPrice: {5}, TradeDate: {6}, ValueDate: {7}, Status: {8}", TradeId, TraderName, CurrencyPair, Notional, Direction, SpotPrice, TradeDate, ValueDate, Status);
        }
    }
}
using System;

namespace Adaptive.ReactiveTrader.Shared.Execution
{
    public class TradeRequestDto
    {
        public string Symbol { get; set; }
        public long QuoteId { get; set; }
        public decimal SpotRate { get; set; }
        public DateTime ValueDate { get; set; }
        public DirectionDto Direction { get; set; }
        public long Notional { get; set; }

        public override string ToString()
        {
            return string.Format("Symbol: {0}, QuoteId: {1}, SpotRate: {2}, ValueDate: {3}, Direction: {4}, Notional: {5}", Symbol, QuoteId, SpotRate, ValueDate, Direction, Notional);
        }
    }
}
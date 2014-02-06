using System;

namespace Adaptive.ReactiveTrader.Contracts.Pricing
{
    public class SpotPrice
    {
        public string Symbol { get; set; }
        public long QuoteId { get; set; }
        public decimal Bid { get; set; }
        public decimal Ask { get; set; }
        public decimal Mid { get; set; }
        public DateTime ValueDate { get; set; }

        public override string ToString()
        {
            return string.Format("Symbol: {0}, QuoteId: {1}, Bid: {2}, Ask: {3}, Mid: {4}, ValueDate: {5}", Symbol, QuoteId, Bid, Ask, Mid, ValueDate);
        }
    }
}
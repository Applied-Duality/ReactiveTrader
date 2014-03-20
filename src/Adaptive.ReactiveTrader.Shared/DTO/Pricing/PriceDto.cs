using System;

namespace Adaptive.ReactiveTrader.Shared.DTO.Pricing
{
    public class PriceDto
    {
        public string Symbol { get; set; }
        public long QuoteId { get; set; }
        public decimal Bid { get; set; }
        public decimal Ask { get; set; }
        public DateTime ValueDate { get; set; }
        public decimal Mid { get; set; }
        public long CreationTimestamp { get; set; }

        public override string ToString()
        {
            return string.Format("Symbol: {0}, QuoteId: {1}, Bid: {2}, Ask: {3}, ValueDate: {4}", Symbol, QuoteId, Bid, Ask, ValueDate);
        }
    }
}
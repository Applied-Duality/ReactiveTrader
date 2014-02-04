using System;

namespace PerfTestDto
{
    public class PerfTestSpotPrice
    {
        public string Symbol { get; set; }
        public long QuoteId { get; set; }
        public decimal Bid { get; set; }
        public decimal Ask { get; set; }
        public decimal Mid { get; set; }
        public DateTime ValueDate { get; set; }
        public long Timestamp { get; set; }
    }
}

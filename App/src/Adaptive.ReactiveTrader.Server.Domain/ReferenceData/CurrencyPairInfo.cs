using Adaptive.ReactiveTrader.Shared.DTO.ReferenceData;

namespace Adaptive.ReactiveTrader.Server.ReferenceData
{
    public class CurrencyPairInfo
    {
        public CurrencyPairDto CurrencyPair { get; private set; }
        public decimal SampleRate { get; private set; }
        public bool Enabled { get; set; }
        public string Comment { get; set; }
        public bool Stale { get; set; }

        public CurrencyPairInfo(CurrencyPairDto currencyPair, decimal sampleRate, bool enabled, string comment)
        {
            CurrencyPair = currencyPair;
            SampleRate = sampleRate;
            Enabled = enabled;
            Comment = comment;
        }
    }
}
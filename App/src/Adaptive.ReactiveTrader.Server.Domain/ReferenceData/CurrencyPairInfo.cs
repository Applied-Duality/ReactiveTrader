using Adaptive.ReactiveTrader.Shared.ReferenceData;

namespace Adaptive.ReactiveTrader.Server.ReferenceData
{
    public class CurrencyPairInfo
    {
        public CurrencyPairDto CurrencyPair { get; private set; }
        public decimal SampleRate { get; private set; }
        public bool Enabled { get; set; }
        public bool Stale { get; set; }

        public CurrencyPairInfo(CurrencyPairDto currencyPair, decimal sampleRate, bool enabled)
        {
            CurrencyPair = currencyPair;
            SampleRate = sampleRate;
            Enabled = enabled;
        }
    }
}
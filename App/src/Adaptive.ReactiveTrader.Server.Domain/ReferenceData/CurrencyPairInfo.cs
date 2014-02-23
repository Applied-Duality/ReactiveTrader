using Adaptive.ReactiveTrader.Shared.ReferenceData;

namespace Adaptive.ReactiveTrader.Server.ReferenceData
{
    public class CurrencyPairInfo
    {
        public CurrencyPairDto CurrencyPair { get; private set; }
        public decimal SampleRate { get; private set; }

        public CurrencyPairInfo(CurrencyPairDto currencyPair, decimal sampleRate)
        {
            CurrencyPair = currencyPair;
            SampleRate = sampleRate;
        }
    }
}
namespace Adaptive.ReactiveTrader.Contracts.ReferenceData
{
    public class CurrencyPairUpdate
    {
        public UpdateType UpdateType { get; set; }
        public CurrencyPair CurrencyPair { get; set; }
    }
}
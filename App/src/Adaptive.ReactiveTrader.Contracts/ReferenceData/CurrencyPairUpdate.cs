namespace Adaptive.ReactiveTrader.Contracts.ReferenceData
{
    public class CurrencyPairUpdate
    {
        UpdateType UpdateType { get; set; }
        CurrencyPair CurrencyPair { get; set; }
    }
}
namespace Adaptive.ReactiveTrader.Contracts
{
    public class CurrencyPairUpdate
    {
        UpdateType UpdateType { get; set; }
        CurrencyPair CurrencyPair { get; set; }
    }
}
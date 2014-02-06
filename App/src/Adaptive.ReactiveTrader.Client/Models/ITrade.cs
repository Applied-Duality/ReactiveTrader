namespace Adaptive.ReactiveTrader.Client.Models
{
    public interface ITrade
    {
        ICurrencyPair CurrencyPair { get; }
    }
}
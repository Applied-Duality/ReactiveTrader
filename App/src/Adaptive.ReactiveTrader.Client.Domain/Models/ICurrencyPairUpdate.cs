using Adaptive.ReactiveTrader.Client.Domain.Repositories;

namespace Adaptive.ReactiveTrader.Client.Domain.Models
{
    public interface ICurrencyPairUpdate
    {
        UpdateType UpdateType { get; }
        ICurrencyPair CurrencyPair { get; }
    }
}
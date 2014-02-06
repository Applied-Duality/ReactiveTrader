using Adaptive.ReactiveTrader.Contracts.Execution;

namespace Adaptive.ReactiveTrader.Client.Models
{
    internal interface ITradeFactory
    {
        ITrade Create(Trade trade);
    }
}
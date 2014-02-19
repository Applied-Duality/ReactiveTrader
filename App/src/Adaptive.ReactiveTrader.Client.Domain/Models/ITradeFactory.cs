using Adaptive.ReactiveTrader.Shared.Execution;

namespace Adaptive.ReactiveTrader.Client.Domain.Models
{
    interface ITradeFactory
    {
        ITrade Create(TradeDto trade);
    }
}
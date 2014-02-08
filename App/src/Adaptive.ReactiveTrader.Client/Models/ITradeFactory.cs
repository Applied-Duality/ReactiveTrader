using Adaptive.ReactiveTrader.Shared.Execution;

namespace Adaptive.ReactiveTrader.Client.Models
{
    internal interface ITradeFactory
    {
        ITrade Create(TradeDto trade);
    }
}
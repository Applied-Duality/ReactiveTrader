using System;

namespace Adaptive.ReactiveTrader.Client.Models
{
    public interface ITrade
    {
        string CurrencyPair { get; }
        Direction Direction { get; }
        long Notional { get; }
        decimal SpotRate { get; }
        TradeStatus TradeStatus { get; }
        DateTime TradeDate { get; }
        long TradeId { get; }
        string TraderName { get; }
        DateTime ValueDate { get; }
    }
}
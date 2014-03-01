using System;
using Adaptive.ReactiveTrader.Shared.UI;

namespace Adaptive.ReactiveTrader.Client.UI.Blotter
{
    public interface ITradeViewModel : IViewModel
    {
        decimal SpotRate { get; }
        string Notional { get; }
        string Direction { get; }
        string CurrencyPair { get; }
        string TradeId { get; }
        DateTime TradeDate { get; }
        string TradeStatus { get; }
        string TraderName { get; }
        DateTime ValueDate { get; }
    }
}

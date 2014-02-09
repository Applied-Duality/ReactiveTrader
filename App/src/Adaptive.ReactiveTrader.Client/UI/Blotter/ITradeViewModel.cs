namespace Adaptive.ReactiveTrader.Client.UI.Blotter
{
    public interface ITradeViewModel : IViewModel
    {
        string ProductType { get; }
        string SpotRate { get; set; }
        string Notional { get; set; }
        string Direction { get; set; }
        string CurrencyPair { get; set; }
        string TradeId { get; }
        string TradeDate { get; }
        string TradeStatus { get; }
        string TraderName { get; }
        string ValueDate { get; }
    }
}

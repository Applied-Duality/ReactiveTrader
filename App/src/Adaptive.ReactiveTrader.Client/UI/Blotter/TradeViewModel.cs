using System.Globalization;
using Adaptive.ReactiveTrader.Client.Models;
using Adaptive.ReactiveTrader.Shared.UI;
using PropertyChanged;

namespace Adaptive.ReactiveTrader.Client.UI.Blotter
{
    [ImplementPropertyChanged]
    public class TradeViewModel : ViewModelBase, ITradeViewModel
    {
        public TradeViewModel(ITrade trade)
        {
            TradeId = trade.TradeId.ToString(CultureInfo.InvariantCulture);
            CurrencyPair = trade.CurrencyPair;
            Direction = trade.Direction == Models.Direction.Buy ? "Buy" : "Sell";
            Notional = string.Format("{0} {1}", trade.Notional.ToString("N", CultureInfo.InvariantCulture), CurrencyPair.Substring(0, 3));
            SpotRate = trade.SpotRate.ToString(CultureInfo.InvariantCulture);
            TradeDate = trade.TradeDate.ToString("g");
            TradeStatus = trade.TradeStatus.ToString();
            TraderName = trade.TraderName;
            ValueDate = trade.ValueDate.ToString("d");
            ProductType = "SPOT";
        }

        public string SpotRate { get; set; }
        public string Notional { get; set; }
        public string Direction { get; set; }
        public string CurrencyPair { get; set; }
        public string TradeId { get; private set; }
        public string TradeDate { get; private set; }
        public string TradeStatus { get; private set; }
        public string TraderName { get; private set; }
        public string ValueDate { get; private set; }
        public string ProductType { get; private set; }
    }
}
using System;
using System.Globalization;
using Adaptive.ReactiveTrader.Client.Domain.Models;
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
            CurrencyPair = trade.CurrencyPair.Substring(0, 3) + " / " + trade.CurrencyPair.Substring(3, 3);
            Direction = trade.Direction == Domain.Models.Direction.Buy ? "Buy" : "Sell";
            Notional = trade.Notional.ToString("N0", CultureInfo.InvariantCulture) + " " + trade.DealtCurrency;
            SpotRate = trade.SpotRate;
            TradeDate = trade.TradeDate;
            TradeStatus = trade.TradeStatus == Domain.Models.TradeStatus.Done ? "Done" : "REJECTED";
            TraderName = trade.TraderName;
            ValueDate = trade.ValueDate;
            DealtCurrency = trade.DealtCurrency;
        }

        public decimal SpotRate { get; set; }
        public string Notional { get; set; }
        public string Direction { get; set; }
        public string CurrencyPair { get; set; }
        public string TradeId { get; private set; }
        public DateTime TradeDate { get; private set; }
        public string TradeStatus { get; private set; }
        public string TraderName { get; private set; }
        public DateTime ValueDate { get; private set; }
        public string DealtCurrency { get; private set; }
    }
}
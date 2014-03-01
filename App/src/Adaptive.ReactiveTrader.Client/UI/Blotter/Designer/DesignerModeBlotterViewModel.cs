using System;
using System.Collections.ObjectModel;
using Adaptive.ReactiveTrader.Shared.UI;

namespace Adaptive.ReactiveTrader.Client.UI.Blotter.Designer
{
    public class DesignerModeBlotterViewModel : ViewModelBase, IBlotterViewModel
    {
        public ObservableCollection<ITradeViewModel> Trades { get; private set; }

        public DesignerModeBlotterViewModel()
        {
            Trades = new ObservableCollection<ITradeViewModel>();

            Trades.Add(new DesignerModeTradeViewModel
            {
                CurrencyPair = "EUR / USD",
                Direction = "Sell",
                Notional = "1,000,000 EUR",
                SpotRate = 1.23456m,
                TradeDate = DateTime.Now,
                ValueDate = DateTime.Now.AddDays(2),
                TradeId = "12834",
                TradeStatus = "Rejected",
                TraderName = "Olivier"
            });

            Trades.Add(new DesignerModeTradeViewModel
            {
                CurrencyPair = "EUR / USD",
                Direction = "Sell",
                Notional = "1,000,000 EUR",
                SpotRate = 1.23456m,
                TradeDate = DateTime.Now,
                ValueDate = DateTime.Now.AddDays(2),
                TradeId = "12834",
                TradeStatus = "Done",
                TraderName = "Olivier"
            });
        }
    }
}
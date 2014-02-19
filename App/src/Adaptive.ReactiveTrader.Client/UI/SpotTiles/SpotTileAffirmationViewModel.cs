using System;
using System.Windows.Input;
using Adaptive.ReactiveTrader.Client.Models;
using Adaptive.ReactiveTrader.Shared.UI;
using PropertyChanged;

namespace Adaptive.ReactiveTrader.Client.UI.SpotTiles
{
    [ImplementPropertyChanged]
    public class SpotTileAffirmationViewModel : ViewModelBase, ISpotTileAffirmationViewModel
    {
        private readonly ITrade _trade;
        private readonly ISpotTileViewModel _parent;
        private readonly DelegateCommand _dismissCommand;

        public SpotTileAffirmationViewModel(ITrade trade, ISpotTileViewModel parent)
        {
            _trade = trade;
            _parent = parent;

            _dismissCommand = new DelegateCommand(OnDismissExecute);
        }

        public string CurrencyPair { get { return _trade.CurrencyPair; } }
        public Direction Direction { get { return _trade.Direction; } }
        public long Notional { get { return _trade.Notional; } }
        public decimal SpotRate { get { return _trade.SpotRate; } }
        public TradeStatus TradeStatus { get { return _trade.TradeStatus; } }
        public DateTime TradeDate { get { return _trade.TradeDate; } }
        public long TradeId { get { return _trade.TradeId; } }
        public string TraderName { get { return _trade.TraderName; } }
        public DateTime ValueDate { get { return _trade.ValueDate; } }
        public ICommand DismissCommand { get { return _dismissCommand; } }

        private void OnDismissExecute()
        {
            _parent.DismissAffirmation();
        }
    }
}

using System.Collections.ObjectModel;

namespace Adaptive.ReactiveTrader.Client.UI.Blotter
{
    public class BlotterViewModel : ViewModelBase, IBlotterViewModel
    {
        public ObservableCollection<ITradeViewModel> Trades { get; private set; }

        public BlotterViewModel()
        {
            Trades = new ObservableCollection<ITradeViewModel>();
        }
    }
}
using System.Windows.Input;
using Adaptive.ReactiveTrader.Client.Models;
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

        public ICommand DismissCommand { get { return _dismissCommand; } }

        private void OnDismissExecute()
        {
            _parent.DismissAffirmation();
        }
    }
}

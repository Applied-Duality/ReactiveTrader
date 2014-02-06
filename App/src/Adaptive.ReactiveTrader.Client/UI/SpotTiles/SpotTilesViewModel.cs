using System.Collections.ObjectModel;

namespace Adaptive.ReactiveTrader.Client.UI.SpotTiles
{
    public class SpotTilesViewModel : ViewModelBase, ISpotTilesViewModel
    {
        public ObservableCollection<ISpotTileViewModel> SpotTiles { get; private set; }

        public SpotTilesViewModel()
        {
            SpotTiles = new ObservableCollection<ISpotTileViewModel>();
        }
    }
}
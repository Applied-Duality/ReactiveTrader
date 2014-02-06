using System.Collections.ObjectModel;

namespace Adaptive.ReactiveTrader.Client.UI.SpotTiles
{
    public interface ISpotTilesViewModel : IViewModel
    {
        ObservableCollection<ISpotTileViewModel> SpotTiles { get; } 
    }
}

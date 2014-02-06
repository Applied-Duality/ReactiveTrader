using Adaptive.ReactiveTrader.Client.UI.Blotter;
using Adaptive.ReactiveTrader.Client.UI.SpotTiles;

namespace Adaptive.ReactiveTrader.Client.UI.Shell
{
    public class ShellViewModel : ViewModelBase, IShellViewModel
    {
        public ISpotTilesViewModel SpotTiles { get; private set; }
        public IBlotterViewModel Blotter { get; private set; }

        public ShellViewModel(ISpotTilesViewModel spotTiles, IBlotterViewModel blotter)
        {
            SpotTiles = spotTiles;
            Blotter = blotter;
        }
    }
}

using Adaptive.ReactiveTrader.Client.UI.Blotter;
using Adaptive.ReactiveTrader.Client.UI.SpotTiles;

namespace Adaptive.ReactiveTrader.Client.UI.Shell
{
    public interface IShellViewModel : IViewModel
    {
        ISpotTilesViewModel SpotTiles { get; }
        IBlotterViewModel Blotter { get; }
    }
}

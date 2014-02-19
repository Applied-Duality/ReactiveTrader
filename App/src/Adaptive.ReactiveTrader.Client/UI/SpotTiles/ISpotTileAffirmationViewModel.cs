using System.Windows.Input;

namespace Adaptive.ReactiveTrader.Client.UI.SpotTiles
{
    public interface ISpotTileAffirmationViewModel : IViewModel
    {
        ICommand DismissCommand { get; }
    }
}

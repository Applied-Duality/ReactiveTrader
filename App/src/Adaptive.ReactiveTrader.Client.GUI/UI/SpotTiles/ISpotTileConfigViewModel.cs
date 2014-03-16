using System.Windows.Input;
using Adaptive.ReactiveTrader.Shared.UI;

namespace Adaptive.ReactiveTrader.Client.UI.SpotTiles
{
    public interface ISpotTileConfigViewModel : IViewModel
    {
        ICommand StandardCommand { get; }
        ICommand DropFrameCommand { get; }
        ICommand ConflateCommand { get; }
        SpotTileSubscriptionMode SubscriptionMode { get; }
    }
}
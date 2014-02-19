using System.Windows.Input;
using Adaptive.ReactiveTrader.Client.Models;

namespace Adaptive.ReactiveTrader.Client.UI.SpotTiles
{
    public interface IOneWayPriceViewModel : IViewModel
    {
        Direction Direction { get; }
        string BigFigures { get; }
        string Pips { get; }
        string TenthOfPip { get; }
        PriceMovement Movement { get; }
        ICommand ExecuteCommand { get; }
        void OnPrice(IExecutablePrice executablePrice);
        void OnStalePrice();
    }
}

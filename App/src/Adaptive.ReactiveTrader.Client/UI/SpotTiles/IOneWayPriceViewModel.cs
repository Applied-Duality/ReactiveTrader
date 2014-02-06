using System.Windows.Input;
using Adaptive.ReactiveTrader.Client.Models;

namespace Adaptive.ReactiveTrader.Client.UI.SpotTiles
{
    public interface IOneWayPriceViewModel : IViewModel
    {
        Direction Direction { get; }
        string Prefix { get; }
        string Big { get; }
        string Suffix { get; }
        ICommand ExecuteCommand { get; }
        void OnPrice(IExecutablePrice executablePrice);
    }
}

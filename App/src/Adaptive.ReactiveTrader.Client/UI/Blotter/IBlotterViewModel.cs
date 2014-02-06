using System.Collections.ObjectModel;

namespace Adaptive.ReactiveTrader.Client.UI.Blotter
{
    public interface IBlotterViewModel : IViewModel
    {
        ObservableCollection<ITradeViewModel> Trades { get; } 
    }
}

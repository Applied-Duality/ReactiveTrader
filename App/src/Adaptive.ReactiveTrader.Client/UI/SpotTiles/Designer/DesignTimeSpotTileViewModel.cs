using Adaptive.ReactiveTrader.Client.Domain.Models;
using Adaptive.ReactiveTrader.Shared.UI;

namespace Adaptive.ReactiveTrader.Client.UI.SpotTiles.Designer
{
    public class DesignTimeSpotTileViewModel : ViewModelBase, ISpotTileViewModel
    {
        public DesignTimeSpotTileViewModel()
        {
            Pricing = new DesignTimeSpotTilePricingViewModel();
            CurrencyPair = "EURUSD";
        }

        public void Dispose()
        {
            throw new System.NotImplementedException();
        }

        public ISpotTilePricingViewModel Pricing { get; private set; }
        public ISpotTileAffirmationViewModel Affirmation { get; private set; }
        public ISpotTileErrorViewModel Error { get; private set; }
        public TileState State { get { return TileState.Pricing;} }
        public string CurrencyPair { get; private set; }

        public void OnTrade(ITrade trade)
        {
            throw new System.NotImplementedException();
        }

        public void OnExecutionError(string message)
        {
            throw new System.NotImplementedException();
        }

        public void DismissAffirmation()
        {
            throw new System.NotImplementedException();
        }

        public void DismissError()
        {
            throw new System.NotImplementedException();
        }
    }
}
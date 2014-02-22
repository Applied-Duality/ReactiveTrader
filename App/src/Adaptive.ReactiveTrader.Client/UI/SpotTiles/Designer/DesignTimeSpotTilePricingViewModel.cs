using System;
using Adaptive.ReactiveTrader.Client.Domain.Models;
using Adaptive.ReactiveTrader.Shared.UI;

namespace Adaptive.ReactiveTrader.Client.UI.SpotTiles.Designer
{
    public class DesignTimeSpotTilePricingViewModel :ViewModelBase, ISpotTilePricingViewModel
    {
        public DesignTimeSpotTilePricingViewModel()
        {
            Bid = new DesignTimeOneWayPriceViewModel(Direction.Sell, "1.23", "45", "6", PriceMovement.Down);
            Ask = new DesignTimeOneWayPriceViewModel(Direction.Buy, "1.23", "46", "7", PriceMovement.Down);
        }

        public void Dispose()
        {

        }

        public string Symbol { get { return "EUR / USD"; } }
        public IOneWayPriceViewModel Bid { get; private set; }
        public IOneWayPriceViewModel Ask { get; private set; }

        public string Notional
        {
            get { return "1000000"; }
            set { }
        }
        public string Spread { get { return "1.0"; } }
        public string DealtCurrency { get { return "EUR"; } }

        public void OnTrade(ITrade trade)
        {
            throw new NotImplementedException();
        }
    }
}
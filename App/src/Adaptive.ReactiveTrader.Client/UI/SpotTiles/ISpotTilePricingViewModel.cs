using System;
using Adaptive.ReactiveTrader.Client.Models;
using Adaptive.ReactiveTrader.Shared.UI;

namespace Adaptive.ReactiveTrader.Client.UI.SpotTiles
{
    public interface ISpotTilePricingViewModel : IViewModel, IDisposable
    {
        string Symbol { get; }
        IOneWayPriceViewModel Bid { get; }
        IOneWayPriceViewModel Ask { get; }
        string Notional { get; set; }
        string Spread { get; }

        void OnTrade(ITrade trade);
    }
}

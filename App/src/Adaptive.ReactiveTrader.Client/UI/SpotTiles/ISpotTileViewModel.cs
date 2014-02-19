using System;
using Adaptive.ReactiveTrader.Client.Models;

namespace Adaptive.ReactiveTrader.Client.UI.SpotTiles
{
    public interface ISpotTileViewModel : IViewModel, IDisposable
    {
        ISpotTilePricingViewModel Pricing { get; }
        ISpotTileAffirmationViewModel Affirmation { get; }
        bool ShowingAffirmation { get; }
        void OnTrade(ITrade trade);
        void DismissAffirmation();
    }
}

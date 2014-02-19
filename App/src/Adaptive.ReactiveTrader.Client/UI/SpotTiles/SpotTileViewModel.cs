using System;
using Adaptive.ReactiveTrader.Client.Models;
using Adaptive.ReactiveTrader.Shared.UI;
using log4net;
using PropertyChanged;

namespace Adaptive.ReactiveTrader.Client.UI.SpotTiles
{
    [ImplementPropertyChanged]
    public class SpotTileViewModel : ViewModelBase, ISpotTileViewModel
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(SpotTileViewModel));
        
        public ISpotTilePricingViewModel Pricing { get; private set; }
        public ISpotTileAffirmationViewModel Affirmation { get; private set; }
        public bool ShowingAffirmation { get; private set; }

        private readonly Func<ITrade, ISpotTileViewModel, ISpotTileAffirmationViewModel> _affirmationFactory;
        private bool _disposed;

        public SpotTileViewModel(ICurrencyPair currencyPair,
            Func<ICurrencyPair, ISpotTileViewModel, ISpotTilePricingViewModel> pricingFactory,
            Func<ITrade, ISpotTileViewModel, ISpotTileAffirmationViewModel> affirmationFactory)
        {
            _affirmationFactory = affirmationFactory;

            Pricing = pricingFactory(currencyPair, this);
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                Pricing.Dispose();
                _disposed = true;
            }
        }

        public void OnTrade(ITrade trade)
        {
            Affirmation = _affirmationFactory(trade, this);
            ShowingAffirmation = true;
        }

        public void DismissAffirmation()
        {
            ShowingAffirmation = false;
            Affirmation = null;
        }
    }
}

using System;
using Adaptive.ReactiveTrader.Client.Domain.Models;
using Adaptive.ReactiveTrader.Shared.UI;
using PropertyChanged;

namespace Adaptive.ReactiveTrader.Client.UI.SpotTiles
{
    [ImplementPropertyChanged]
    public class SpotTileViewModel : ViewModelBase, ISpotTileViewModel
    {
        public ISpotTilePricingViewModel Pricing { get; private set; }
        public ISpotTileAffirmationViewModel Affirmation { get; private set; }
        public bool ShowingAffirmation { get; private set; }
        public string CurrencyPair { get; private set; }

        private readonly Func<ITrade, ISpotTileViewModel, ISpotTileAffirmationViewModel> _affirmationFactory;
        private bool _disposed;

        public SpotTileViewModel(ICurrencyPair currencyPair,
            Func<ICurrencyPair, ISpotTileViewModel, ISpotTilePricingViewModel> pricingFactory,
            Func<ITrade, ISpotTileViewModel, ISpotTileAffirmationViewModel> affirmationFactory)
        {
            _affirmationFactory = affirmationFactory;

            Pricing = pricingFactory(currencyPair, this);
            CurrencyPair = currencyPair.Symbol;
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

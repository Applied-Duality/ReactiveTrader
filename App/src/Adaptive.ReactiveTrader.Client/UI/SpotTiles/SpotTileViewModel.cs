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
        public ISpotTileErrorViewModel Error { get; private set; }
        public TileState State { get; private set; }

        public string CurrencyPair { get; private set; }

        private readonly Func<ITrade, ISpotTileViewModel, ISpotTileAffirmationViewModel> _affirmationFactory;
        private readonly Func<string, ISpotTileViewModel, ISpotTileErrorViewModel> _errorFactory;
        private bool _disposed;

        public SpotTileViewModel(ICurrencyPair currencyPair,
            Func<ICurrencyPair, ISpotTileViewModel, ISpotTilePricingViewModel> pricingFactory,
            Func<ITrade, ISpotTileViewModel, ISpotTileAffirmationViewModel> affirmationFactory,
            Func<string, ISpotTileViewModel, ISpotTileErrorViewModel> errorFactory)
        {
            _affirmationFactory = affirmationFactory;
            _errorFactory = errorFactory;

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
            State = TileState.Affirmation;
        }

        public void OnExecutionError(string message)
        {
            Error = _errorFactory(message, this);
            State = TileState.Error;
        }

        public void DismissAffirmation()
        {
            State = TileState.Pricing;
            Affirmation = null;
        }

        public void DismissError()
        {
            State = TileState.Pricing;
            Error = null;
        }
    }

    public enum TileState
    {
        Pricing,
        Affirmation,
        Error
    }
}

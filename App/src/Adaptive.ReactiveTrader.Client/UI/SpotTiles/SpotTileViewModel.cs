using System;
using System.Reactive.Linq;
using Adaptive.ReactiveTrader.Client.Models;
using log4net;

namespace Adaptive.ReactiveTrader.Client.UI.SpotTiles
{
    public class SpotTileViewModel : ViewModelBase, ISpotTileViewModel
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(SpotTileViewModel));

        public IOneWayPriceViewModel Bid { get; private set; }
        public IOneWayPriceViewModel Ask { get; private set; }
        public string Notional { get; set; }
        public string Spread { get; private set; }

        private readonly ICurrencyPair _currencyPair;
        private bool _disposed;
        IDisposable _priceSubscription;

        public SpotTileViewModel(ICurrencyPair currencyPair, Func<Direction, ISpotTileViewModel, IOneWayPriceViewModel> oneWayPriceFactory)
        {
            _currencyPair = currencyPair;

            Bid = oneWayPriceFactory(Direction.Sell, this);
            Ask = oneWayPriceFactory(Direction.Buy, this);
            Notional = "1000000";

            SubscribeForPrices();
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
            }
        }

        public string Symbol { get { return _currencyPair.Symbol; } }
        
        private void SubscribeForPrices()
        {
            _priceSubscription = _currencyPair.Prices
                .ObserveOnDispatcher()
                .Subscribe(OnPrice, error => Log.Error("Failed to get prices"));
        }

        private void OnPrice(IPrice price)
        {
            if (price.IsStale)
            {
                Bid.OnStalePrice();
                Ask.OnStalePrice();
                Spread = string.Empty;
            }
            else
            {
                Bid.OnPrice(price.Bid);
                Ask.OnPrice(price.Ask);
                Spread = PriceFormatter.GetFormattedSpread(price.Spread, _currencyPair.RatePrecision, _currencyPair.PipsPosition);
            }

            // Olivier: looks like fody is not working, not sure why... remove this hack when it's fixed
            OnPropertyChangedManual("Spread");
        }

    }
}

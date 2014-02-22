using System;
using System.Net.Configuration;
using System.Reactive.Linq;
using Adaptive.ReactiveTrader.Client.Domain.Models;
using Adaptive.ReactiveTrader.Shared.UI;
using log4net;
using PropertyChanged;

namespace Adaptive.ReactiveTrader.Client.UI.SpotTiles
{
    [ImplementPropertyChanged]
    public class SpotTilePricingViewModel : ViewModelBase, ISpotTilePricingViewModel
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(SpotTilePricingViewModel));
        public IOneWayPriceViewModel Bid { get; private set; }
        public IOneWayPriceViewModel Ask { get; private set; }
        public string Notional { get; set; }
        public string Spread { get; private set; }

        private readonly ICurrencyPair _currencyPair;
        private readonly ISpotTileViewModel _parent;
        private bool _disposed;
        private IDisposable _priceSubscription;

        public SpotTilePricingViewModel(ICurrencyPair currencyPair, ISpotTileViewModel parent,
            Func<Direction, ISpotTilePricingViewModel, IOneWayPriceViewModel> oneWayPriceFactory)
        {
            _currencyPair = currencyPair;
            _parent = parent;

            Bid = oneWayPriceFactory(Direction.Sell, this);
            Ask = oneWayPriceFactory(Direction.Buy, this);
            Notional = "1000000";

            SubscribeForPrices();
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _priceSubscription.Dispose();
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
        }

        public void OnTrade(ITrade trade)
        {
            _parent.OnTrade(trade);
        }
    }
}

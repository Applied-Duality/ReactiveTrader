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
        private readonly ICurrencyPair _currencyPair;
        private readonly Func<Direction, IOneWayPriceViewModel> _oneWayPriceFactory;
        private bool _disposed;
        IDisposable _priceSubscription;

        public SpotTileViewModel(ICurrencyPair currencyPair, Func<Direction, IOneWayPriceViewModel> oneWayPriceFactory)
        {
            _currencyPair = currencyPair;
            _oneWayPriceFactory = oneWayPriceFactory;

            Bid = oneWayPriceFactory(Direction.Buy);
            Ask = oneWayPriceFactory(Direction.Sell);
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
            Bid.OnPrice(price.Bid);
            Ask.OnPrice(price.Ask);
        }
    }
}

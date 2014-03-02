using System;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Threading;
using Adaptive.ReactiveTrader.Client.Domain.Models;
using Adaptive.ReactiveTrader.Client.Instrumentation;
using Adaptive.ReactiveTrader.Shared.Extensions;
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
        public string DealtCurrency { get; private set; }

        private readonly ICurrencyPair _currencyPair;
        private readonly ISpotTileViewModel _parent;
        private readonly IPriceLatencyRecorder _priceLatencyRecorder;
        private bool _disposed;
        private IDisposable _priceSubscription;

        public SpotTilePricingViewModel(ICurrencyPair currencyPair, ISpotTileViewModel parent,
            Func<Direction, ISpotTilePricingViewModel, IOneWayPriceViewModel> oneWayPriceFactory,
            IPriceLatencyRecorder priceLatencyRecorder)
        {
            _currencyPair = currencyPair;
            _parent = parent;
            _priceLatencyRecorder = priceLatencyRecorder;

            Bid = oneWayPriceFactory(Direction.Sell, this);
            Ask = oneWayPriceFactory(Direction.Buy, this);
            Notional = "1000000";
            DealtCurrency = currencyPair.BaseCurrency;

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

        public string Symbol { get { return String.Format("{0} / {1}", _currencyPair.BaseCurrency, _currencyPair.CounterCurrency); } }

        private void SubscribeForPrices()
        {
            // 3 different options here: 
            //  - observe everything (ObserveOnDispatcher)
            //  - conflate 
            //  - observe only the most recent update (ObserveLatestOn)

            _priceSubscription = _currencyPair.Prices
                //.ObserveLatestOn(DispatcherScheduler.Current) 
                //.ObserveOnDispatcher()
                .Conflate(TimeSpan.FromMilliseconds(250), DispatcherScheduler.Current)
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
                _priceLatencyRecorder.RecordProcessingTime(price.ElpasedTimeSinceCreated);
            }
        }

        public void OnTrade(ITrade trade)
        {
            _parent.OnTrade(trade);
        }
    }
}

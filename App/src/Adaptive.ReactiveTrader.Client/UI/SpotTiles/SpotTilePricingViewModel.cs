using System;
using System.Reactive.Concurrency;
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
        public PriceMovement Movement { get; private set; }
        public string SpotDate { get; private set; }

        private readonly ICurrencyPair _currencyPair;
        private readonly ISpotTileViewModel _parent;
        private readonly IPriceLatencyRecorder _priceLatencyRecorder;
        private bool _disposed;
        private IDisposable _priceSubscription;
        private decimal? _previousRate;

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
            SpotDate = "SP";

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
                _previousRate = null;
                Movement = PriceMovement.None;
                SpotDate = "SP";
            }
            else
            {
                if (_previousRate.HasValue)
                {   // todo - should be using a mid price
                    if (price.Bid.Rate > _previousRate.Value) 
                        Movement = PriceMovement.Up;
                    else if (price.Bid.Rate < _previousRate.Value)
                        Movement = PriceMovement.Down;
                    else
                        Movement = PriceMovement.None;
                }
                _previousRate = price.Bid.Rate;

                Bid.OnPrice(price.Bid);
                Ask.OnPrice(price.Ask);
                Spread = PriceFormatter.GetFormattedSpread(price.Spread, _currencyPair.RatePrecision, _currencyPair.PipsPosition);
                _priceLatencyRecorder.RecordProcessingTime(price.ElpasedTimeSinceCreated);
                SpotDate = "SP. " + price.ValueDate.ToString("dd MMM");
            }
        }

        public void OnTrade(ITrade trade)
        {
            _parent.OnTrade(trade);
        }

        public void OnExecutionError(string message)
        {
            _parent.OnExecutionError(message);
        }
    }
}

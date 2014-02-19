using System;
using System.Reactive.Linq;
using System.Windows.Input;
using Adaptive.ReactiveTrader.Client.Models;
using log4net;
using PropertyChanged;

namespace Adaptive.ReactiveTrader.Client.UI.SpotTiles
{
    [ImplementPropertyChanged]
    public class OneWayPriceViewModel : ViewModelBase, IOneWayPriceViewModel
    {
        private readonly ISpotTilePricingViewModel _parent;
        private static readonly ILog Log = LogManager.GetLogger(typeof(OneWayPriceViewModel));

        private readonly DelegateCommand _executeCommand;
        private IExecutablePrice _executablePrice;
        private decimal? _previousRate;

        public Direction Direction { get; private set; }
        public string BigFigures { get; private set; }
        public string Pips { get; private set; }
        public string TenthOfPip { get; private set; }
        public PriceMovement Movement { get; private set; }
        
        public OneWayPriceViewModel(Direction direction, ISpotTilePricingViewModel parent)
        {
            _parent = parent;
            Direction = direction;

            _executeCommand = new DelegateCommand(OnExecute, CanExecute);
        }

        #region ExecuteCommand
        public ICommand ExecuteCommand { get { return _executeCommand; } }

        private bool CanExecute()
        {
            return _executablePrice != null;
        }

        private void OnExecute()
        {
            long notional;
            if (!long.TryParse(_parent.Notional, out notional))
            {
                // TODO handle notional validation properly
                return;
            }

            _executablePrice.Execute(notional)
                .ObserveOnDispatcher()
                .Subscribe(OnExecuted,
                    error => Log.Error("Failed to execute trade", error));
        }
        #endregion

        public void OnPrice(IExecutablePrice executablePrice)
        {
            _executablePrice = executablePrice;
            if (_previousRate.HasValue)
            {
                Movement = _executablePrice.Rate > _previousRate.Value 
                    ? PriceMovement.Up 
                    : PriceMovement.Down;
            }
            var formattedPrice = PriceFormatter.GetFormattedPrice(_executablePrice.Rate,
                executablePrice.Parent.CurrencyPair.RatePrecision, executablePrice.Parent.CurrencyPair.PipsPosition);

            _previousRate = _executablePrice.Rate;

            BigFigures = formattedPrice.BigFigures;
            Pips = formattedPrice.Pips;
            TenthOfPip = formattedPrice.TenthOfPip;

            _executeCommand.RaiseCanExecuteChanged();
        }

        public void OnStalePrice()
        {
            _executablePrice = null;
            _previousRate = null;

            BigFigures = string.Empty;
            Pips = string.Empty;
            TenthOfPip = string.Empty;
            Movement = PriceMovement.None;

            _executeCommand.RaiseCanExecuteChanged();
        }

        private void OnExecuted(ITrade trade)
        {
            Log.Info("Trade executed");
            _parent.OnTrade(trade);
        }
    }

    public enum PriceMovement
    {
        None,
        Down,
        Up
    }
}

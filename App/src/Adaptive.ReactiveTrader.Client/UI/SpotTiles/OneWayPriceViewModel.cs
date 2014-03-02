using System;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Input;
using Adaptive.ReactiveTrader.Client.Domain.Models;
using Adaptive.ReactiveTrader.Shared.UI;
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
        public bool IsExecuting { get; private set; }
        
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
            return _executablePrice != null && !IsExecuting;
        }

        private void OnExecute()
        {
            long notional;
            if (!long.TryParse(_parent.Notional, out notional))
            {
                // TODO handle notional validation properly
                return;
            }
            IsExecuting = true;
            _executablePrice.Execute(notional, _parent.DealtCurrency)
                .ObserveOnDispatcher()
                .Subscribe(OnExecuted,
                    OnExecutionError);    
        }

        private void OnExecutionError(Exception exception)
        {
            if (exception is TimeoutException)
            {
                MessageBox.Show("No response was received from the server, the execution status is unknown.\nPlease contact your sales representative.", "Execution timeout", MessageBoxButton.OK, MessageBoxImage.Error);
                Log.Error("Trade execution request timed out.");
            }
            else
            {
                MessageBox.Show("An error occured while processing the trade request.", "Execution error", MessageBoxButton.OK, MessageBoxImage.Error);
                Log.Error("An error occured while processing the trade request.", exception);
            }
            IsExecuting = false;
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
            IsExecuting = false;
        }
    }

    public enum PriceMovement
    {
        None,
        Down,
        Up
    }
}

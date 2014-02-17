using System;
using System.Reactive.Linq;
using System.Windows.Input;
using Adaptive.ReactiveTrader.Client.Models;
using log4net;

namespace Adaptive.ReactiveTrader.Client.UI.SpotTiles
{
    public class OneWayPriceViewModel : ViewModelBase, IOneWayPriceViewModel
    {
        private readonly ISpotTileViewModel _spotTileViewModel;
        private static readonly ILog Log = LogManager.GetLogger(typeof(OneWayPriceViewModel));

        private readonly DelegateCommand _executeCommand;
        private IExecutablePrice _executablePrice;
        private decimal? _previousRate;

        public Direction Direction { get; private set; }
        public string BigFigures { get; private set; }
        public string Pips { get; private set; }
        public string TenthOfPip { get; private set; }
        public PriceMovement Movement { get; private set; }
        
        public OneWayPriceViewModel(Direction direction, ISpotTileViewModel spotTileViewModel)
        {
            _spotTileViewModel = spotTileViewModel;
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
            if (!long.TryParse(_spotTileViewModel.Notional, out notional))
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

            OnPropertyChangedManual("BigFigures");
            OnPropertyChangedManual("Pips");
            OnPropertyChangedManual("TenthOfPip");
            OnPropertyChangedManual("Movement");

            _executeCommand.RaiseCanExecuteChanged();
        }

        

        private static void OnExecuted(ITrade trade)
        {
            Log.Info("Trade executed");
        }
    }

    public enum PriceMovement
    {
        None,
        Down,
        Up
    }
}

using System;
using System.Reactive.Linq;
using System.Windows.Input;
using Adaptive.ReactiveTrader.Client.Models;
using log4net;

namespace Adaptive.ReactiveTrader.Client.UI.SpotTiles
{
    public class OneWayPriceViewModel : ViewModelBase, IOneWayPriceViewModel
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(OneWayPriceViewModel));

        private readonly DelegateCommand _executeCommand;
        private IExecutablePrice _executablePrice;

        public Direction Direction { get; private set; }
        public string BigFigures { get; private set; }
        public string Pips { get; private set; }
        public string TenthOfPip { get; private set; }
        
        public OneWayPriceViewModel(Direction direction)
        {
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
            _executablePrice.Execute(1000000) // TODO bind notional
                .ObserveOnDispatcher()
                .Subscribe(OnExecuted,
                    error => Log.Error("Failed to execute trade", error));
        }
        #endregion

        public void OnPrice(IExecutablePrice executablePrice)
        {
            _executablePrice = executablePrice;

            var formattedPrice = PriceFormatter.GetFormattedPrice(_executablePrice.Rate,
                executablePrice.Parent.CurrencyPair.RatePrecision, executablePrice.Parent.CurrencyPair.PipsPosition);

            BigFigures = formattedPrice.BigFigures;
            Pips = formattedPrice.Pips;
            TenthOfPip = formattedPrice.TenthOfPip;

            OnPropertyChangedManual("BigFigures");
            OnPropertyChangedManual("Pips");
            OnPropertyChangedManual("TenthOfPip");

            _executeCommand.RaiseCanExecuteChanged();
        }

        private static void OnExecuted(ITrade trade)
        {
            Log.Info("Trade executed");
        }
    }
}

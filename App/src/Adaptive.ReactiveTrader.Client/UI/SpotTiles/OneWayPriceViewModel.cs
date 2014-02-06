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
        public Direction Direction { get; private set; }
        public string Prefix { get; private set; }
        public string Big { get; private set; }
        public string Suffix { get; private set; }
        private readonly DelegateCommand _executeCommand;
        private IExecutablePrice _executablePrice;

        public OneWayPriceViewModel(Direction direction)
        {
            Direction = direction;

            _executeCommand = new DelegateCommand(OnExecute, CanExecute);
        }

        #region ExecuteCommand
        public ICommand ExecuteCommand { get { return _executeCommand; } }

        private bool CanExecute()
        {
            if (_executablePrice != null)
            {
                return true;
            }
            return false;
        }

        private void OnExecute()
        {
            _executablePrice.Execute()
                .ObserveOnDispatcher()
                .Subscribe(OnExecuted,
                    error => Log.Error("Failed to execute trade", error));
        }
        #endregion

        public void OnPrice(IExecutablePrice executablePrice)
        {
            _executablePrice = executablePrice;
            _executeCommand.RaiseCanExecuteChanged();
        }

        private static void OnExecuted(ITrade trade)
        {
            Log.Info("Trade executed");
        }
    }
}

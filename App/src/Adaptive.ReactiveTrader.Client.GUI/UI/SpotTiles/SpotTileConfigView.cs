using System;
using System.Windows.Input;
using Adaptive.ReactiveTrader.Shared.UI;

namespace Adaptive.ReactiveTrader.Client.UI.SpotTiles
{
    public class SpotTileConfigViewModel : ViewModelBase,  ISpotTileConfigViewModel
    {
        private readonly DelegateCommand _standardCommand;
        private readonly DelegateCommand _dropFrameCommand;
        private readonly DelegateCommand _conflateCommand;

        public SpotTileConfigViewModel()
        {
            _standardCommand = new DelegateCommand(ExecuteStandardCommand, CanExecuteStandardCommand);
            _dropFrameCommand = new DelegateCommand(ExecuteDropFrameCommand, CanExecuteDropFrameCommand);
            _conflateCommand = new DelegateCommand(ExecuteConflateCommand, CanExecuteConflateCommand);

        }

        public ICommand StandardCommand
        {
            get { return _standardCommand; }
        }

        public ICommand DropFrameCommand
        {
            get { return _dropFrameCommand; }
        }

        public ICommand ConflateCommand
        {
            get { return _conflateCommand; }
        }

        public SpotTileSubscriptionMode SubscriptionMode { get; private set; }

        private void ExecuteStandardCommand()
        {
            SubscriptionMode = SpotTileSubscriptionMode.OnDispatcher;
            RaiseCanExecuteChanged();
        }

        private bool CanExecuteStandardCommand()
        {
            return SubscriptionMode != SpotTileSubscriptionMode.OnDispatcher;
        }

        private void ExecuteDropFrameCommand()
        {
            SubscriptionMode = SpotTileSubscriptionMode.ObserveLatestOnDispatcher;
            RaiseCanExecuteChanged();
        }

        private bool CanExecuteDropFrameCommand()
        {
            return SubscriptionMode != SpotTileSubscriptionMode.ObserveLatestOnDispatcher;
        }

        private void ExecuteConflateCommand()
        {
            SubscriptionMode = SpotTileSubscriptionMode.Conflate;
            RaiseCanExecuteChanged();
        }

        private bool CanExecuteConflateCommand()
        {
            return SubscriptionMode != SpotTileSubscriptionMode.Conflate;
        }

        private void RaiseCanExecuteChanged()
        {
            _standardCommand.RaiseCanExecuteChanged();
            _dropFrameCommand.RaiseCanExecuteChanged();
            _conflateCommand.RaiseCanExecuteChanged();
        }
    }
}
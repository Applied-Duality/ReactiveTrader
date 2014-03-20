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
        private readonly DelegateCommand _constantRateCommand;

        public SpotTileConfigViewModel()
        {
            _standardCommand = CreateCommand(SpotTileSubscriptionMode.OnDispatcher);
            _dropFrameCommand = CreateCommand(SpotTileSubscriptionMode.ObserveLatestOnDispatcher);
            _conflateCommand = CreateCommand(SpotTileSubscriptionMode.Conflate);
            _constantRateCommand = CreateCommand(SpotTileSubscriptionMode.ConstantRate);
        }

        private DelegateCommand CreateCommand(SpotTileSubscriptionMode spotTileSubscriptionMode)
        {
            return new DelegateCommand(() => Execute(spotTileSubscriptionMode), () => CanExecute(spotTileSubscriptionMode));
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

        public ICommand ConstantRateCommand
        {
            get { return _constantRateCommand; }
        }

        public SpotTileSubscriptionMode SubscriptionMode { get; private set; }

        private void Execute(SpotTileSubscriptionMode spotTileSubscriptionMode)
        {
            SubscriptionMode = spotTileSubscriptionMode;
            RaiseCanExecuteChanged();
        }

        private bool CanExecute(SpotTileSubscriptionMode spotTileSubscriptionMode)
        {
            return SubscriptionMode != spotTileSubscriptionMode;
        }

        private void RaiseCanExecuteChanged()
        {
            _standardCommand.RaiseCanExecuteChanged();
            _dropFrameCommand.RaiseCanExecuteChanged();
            _conflateCommand.RaiseCanExecuteChanged();
            _constantRateCommand.RaiseCanExecuteChanged();
        }
    }
}
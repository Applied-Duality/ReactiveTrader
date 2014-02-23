using System;
using System.Reactive.Linq;
using Adaptive.ReactiveTrader.Client.Domain;
using Adaptive.ReactiveTrader.Client.Domain.Transport;
using Adaptive.ReactiveTrader.Client.Instrumentation;
using Adaptive.ReactiveTrader.Shared.UI;
using log4net;

namespace Adaptive.ReactiveTrader.Client.UI.Connectivity
{
    class ConnectivityStatusViewModel : ViewModelBase, IConnectivityStatusViewModel
    {
        private readonly IPriceLatencyRecorder _priceLatencyRecorder;
        private static readonly ILog Log = LogManager.GetLogger(typeof(ConnectivityStatusViewModel));

        public ConnectivityStatusViewModel(IReactiveTrader reactiveTrader, IPriceLatencyRecorder priceLatencyRecorder)
        {
            _priceLatencyRecorder = priceLatencyRecorder;
            reactiveTrader.ConnectionStatus
                .ObserveOnDispatcher()
                .Subscribe(
                OnStatusChange,
                ex => Log.Error("An error occured within the connection status stream.", ex));

            Observable
                .Timer(TimeSpan.FromSeconds(1))
                .Repeat()
                .ObserveOnDispatcher()
                .Subscribe(OnTimerTick);
        }

        private void OnTimerTick(long _)
        {
            var current = _priceLatencyRecorder.GetCurrentAndReset();
            UiLatency = (int)current.Item1.TotalMilliseconds;
            Throughput = current.Item2;
        }

        private void OnStatusChange(ConnectionInfo connectionInfo)
        {
            switch (connectionInfo.ConnectionStatus)
            {
                case ConnectionStatus.Uninitialized:
                case ConnectionStatus.Connecting:
                    Status = string.Format("Connecting to {0} ...", connectionInfo.Server);
                    break;
                case ConnectionStatus.Reconnected:
                case ConnectionStatus.Connected:
                    Status = string.Format("Connected to {0}", connectionInfo.Server);
                    break;
                case ConnectionStatus.ConnectionSlow:
                    Status = string.Format("Slow connection detected with {0}", connectionInfo.Server);
                    break;
                case ConnectionStatus.Reconnecting:
                    Status = string.Format("Reconnecting to {0} ...", connectionInfo.Server);
                    break;
                case ConnectionStatus.Closed:
                    Status = string.Format("Disconnected from {0}", connectionInfo.Server);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public string Status { get; private set; }
        public long UiLatency { get; private set; }
        public long Throughput { get; private set; }
    }
}
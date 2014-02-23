using System;
using System.Reactive.Linq;
using Adaptive.ReactiveTrader.Client.Domain;
using Adaptive.ReactiveTrader.Client.Domain.Transport;
using Adaptive.ReactiveTrader.Shared.UI;
using log4net;

namespace Adaptive.ReactiveTrader.Client.UI.Connectivity
{
    class ConnectivityStatusViewModel : ViewModelBase, IConnectivityStatusViewModel
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(ConnectivityStatusViewModel));

        public ConnectivityStatusViewModel(IReactiveTrader reactiveTrader)
        {
            reactiveTrader.ConnectionStatus
                .ObserveOnDispatcher()
                .Subscribe(
                OnStatusChange,
                ex => Log.Error("An error occured within the connection status stream.", ex));
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
    }
}
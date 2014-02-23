using Adaptive.ReactiveTrader.Client.Domain.Transport;

namespace Adaptive.ReactiveTrader.Client.Domain
{
    public class ConnectionInfo
    {
        public ConnectionStatus ConnectionStatus { get; private set; }
        public string Server { get; private set; }

        public ConnectionInfo(ConnectionStatus connectionStatus, string server)
        {
            ConnectionStatus = connectionStatus;
            Server = server;
        }
    }
}
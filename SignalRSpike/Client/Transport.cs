using System.Threading.Tasks;
using Dto;
using log4net;
using Microsoft.AspNet.SignalR.Client;

namespace Client
{
    public class Transport : ITransport
    {
        private IHubProxy _tradingServiceHub;

        private static readonly ILog Log = LogManager.GetLogger(typeof(Transport));

        public async Task Initialize(string address, string userName)
        {
            var hubConnection = new HubConnection(address);
            hubConnection.Headers.Add(ServiceConstants.Server.UsernameHeader,userName);
            _tradingServiceHub = hubConnection.CreateHubProxy(ServiceConstants.Server.TradingServiceHub);

            Log.InfoFormat("Connecting to server {0} with user {1}...", address, userName);

            await hubConnection.Start();
        }

        public IHubProxy HubProxy
        {
            get { return _tradingServiceHub; }
        }
    }
}
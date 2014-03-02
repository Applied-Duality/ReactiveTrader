using System.Threading.Tasks;
using Adaptive.ReactiveTrader.Server.Transport;
using Adaptive.ReactiveTrader.Shared;
using log4net;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace Adaptive.ReactiveTrader.Server.Blotter
{
    [HubName(ServiceConstants.Server.BlotterHub)]
    public class BlotterHub : Hub
    {
        private readonly ITradeRepository _tradeRepository;
        private readonly IContextHolder _contextHolder;
        private static readonly ILog Log = LogManager.GetLogger(typeof(BlotterHub));
        public const string BlotterGroupName = "AllTrades";

        public BlotterHub(ITradeRepository tradeRepository, IContextHolder contextHolder)
        {
            _tradeRepository = tradeRepository;
            _contextHolder = contextHolder;
        }

        [HubMethodName(ServiceConstants.Server.SubscribeTrades)]
        public async Task SubscribeTrades()
        {
            _contextHolder.BlotterHubClients = Clients;

            var user = UserName;
            Log.InfoFormat("Received trade subscription from user {0}", user);

            // add client to the trade notification group
            await Groups.Add(Context.ConnectionId, BlotterGroupName);
            Log.InfoFormat("Connection {0} of user {1} added to group '{2}'", Context.ConnectionId, user, BlotterGroupName);

            var trades = _tradeRepository.GetAllTrades();
            await Clients.Caller.OnNewTrade(trades);
            Log.InfoFormat("Snapshot published to {0}", Context.ConnectionId);
        }

        [HubMethodName(ServiceConstants.Server.UnsubscribeTrades)]
        public async Task UnsubscribeTrades()
        {
            Log.InfoFormat("Received unsubscription request for trades from connection {0}", Context.ConnectionId);

            // remove client from the blotter group
            await Groups.Remove(Context.ConnectionId, BlotterGroupName);
            Log.InfoFormat("Connection {0} removed from group '{1}'", Context.ConnectionId, BlotterGroupName);
        }

        private string UserName
        {
            get { return Context.Headers[ServiceConstants.Server.UsernameHeader]; }
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Adaptive.ReactiveTrader.Contracts;
using Adaptive.ReactiveTrader.Contracts.Pricing;
using log4net;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace Adaptive.ReactiveTrader.Server.SignalR
{
    [HubName(ServiceConstants.Server.TradingServiceHub)]
    public class TradingServiceHub : Hub
    {
        private readonly IPriceLastValueCache _priceLastValueCache;
        private readonly ICurrencyPairRepository _currencyPairRepository;
        private readonly IExecutionService _executionService;
        private readonly ITradeRepository _tradeRepository;
        private readonly IContextHolder _contextHolder;
        public const string PriceStreamGroupPattern = "Pricing/{0}";
        public const string BlotterGroupName = "AllTrades";

        private static readonly ILog Log = LogManager.GetLogger(typeof(TradingServiceHub));

        public TradingServiceHub(
            IPriceLastValueCache priceLastValueCache,
            ICurrencyPairRepository currencyPairRepository,
            IExecutionService executionService,
            ITradeRepository tradeRepository,
            IContextHolder contextHolder)
        {
            _priceLastValueCache = priceLastValueCache;
            _currencyPairRepository = currencyPairRepository;
            _executionService = executionService;
            _tradeRepository = tradeRepository;
            _contextHolder = contextHolder;
        }

        [HubMethodName(ServiceConstants.Server.SubscribePriceStream)]
        public async Task SubscribePriceStream(PriceSubscriptionRequest request)
        {
            _contextHolder.Context = Clients;

            Log.InfoFormat("Received subscription request {0} from connection {1}", request, Context.ConnectionId);

            if (!_currencyPairRepository.Exists(request.CurrencyPair))
            {
                Log.WarnFormat("Received a subscription request for an invalid currency pair '{0}', it was ignored.", request.CurrencyPair);
                return;
            }

            // add client to this group
            var groupName = string.Format(PriceStreamGroupPattern, request.CurrencyPair);
            await Groups.Add(Context.ConnectionId, groupName);
            Log.InfoFormat("Connection {0} added to group '{1}'", Context.ConnectionId, groupName);

            // send current price to client
            var lastValue = _priceLastValueCache.GetLastValue(request.CurrencyPair);
            await Clients.Caller.OnNewPrice(lastValue);
            Log.InfoFormat("Snapshot published to {0}: {1}", Context.ConnectionId, lastValue);
        }

        [HubMethodName(ServiceConstants.Server.UnsubscribePriceStream)]
        public async Task UnsubscribePriceStream(PriceSubscriptionRequest request)
        {
            Log.InfoFormat("Received unsubscription request {0} from connection {1}", request, Context.ConnectionId);

            if (!_currencyPairRepository.Exists(request.CurrencyPair))
            {
                Log.WarnFormat("Received an unsubscription request for an invalid currency pair '{0}', it was ignored.", request.CurrencyPair);
                return;
            }

            // remove client from the group
            var groupName = string.Format(PriceStreamGroupPattern, request.CurrencyPair);
            await Groups.Remove(Context.ConnectionId, groupName);
            Log.InfoFormat("Connection {0} removed from group '{1}'", Context.ConnectionId, groupName);
        }

        [HubMethodName(ServiceConstants.Server.Execute)]
        public SpotTrade Execute(SpotTradeRequest tradeRequest)
        {
            var user = UserName;
            Log.InfoFormat("Received trade request {0} from user {1}", tradeRequest, user);

            var trade = _executionService.Execute(tradeRequest, user);
            Log.InfoFormat("Trade executed: {0}", trade);

            return trade;
        }

        [HubMethodName(ServiceConstants.Server.SubscribeTrades)]
        public async Task SubscribeTrades()
        {
            var user = UserName;
            Log.InfoFormat("Received trade subscription from user {0}", user);

            // add client to the trade notification group
            await Groups.Add(Context.ConnectionId, BlotterGroupName);
            Log.InfoFormat("Connection {0} of user {1} added to group '{2}'", Context.ConnectionId, user, BlotterGroupName);

            var trades = _tradeRepository.GetAllTrades();
            foreach (var spotTrade in trades)
            {
                // TODO implement blotter snapshot properly
                await Clients.Caller.OnNewTrade(spotTrade);
                Log.InfoFormat("Snapshot published to {0}: {1}", Context.ConnectionId, spotTrade);
            }
        }

        [HubMethodName(ServiceConstants.Server.UnsubscribeTrades)]
        public async Task UnsubscribeTrades()
        {
            Log.InfoFormat("Received unsubscription request for trades from connection {0}", Context.ConnectionId);

            // remove client from the blotter group
            await Groups.Remove(Context.ConnectionId, BlotterGroupName);
            Log.InfoFormat("Connection {0} removed from group '{1}'", Context.ConnectionId, BlotterGroupName);
        }

        [HubMethodName(ServiceConstants.Server.GetCurrencyPairs)]
        public IEnumerable<CurrencyPair> GetCurrencyPairs()
        {
            Log.InfoFormat("Received request for currency pairs from connection {0}", Context.ConnectionId);

            var currencyPairs = _currencyPairRepository.GetAllCurrencyPairs().ToList();
            Log.InfoFormat("Sending {0} currency pairs to {1}'", currencyPairs.Count, Context.ConnectionId);

            return currencyPairs;
        }

        public override Task OnDisconnected()
        {
            Log.InfoFormat("Client disconnected: {0}", Context.ConnectionId);
            return base.OnDisconnected();
        }

        public override Task OnReconnected()
        {
            Log.InfoFormat("Client reconnected: {0}", Context.ConnectionId);
            return base.OnReconnected();
        }

        public override Task OnConnected()
        {
            Log.InfoFormat("Client connected with connectionID {0}, UserName: {1}, Transport {2}", Context.ConnectionId, UserName, Context.QueryString["transport"]);
            return base.OnConnected();
        }

        private string UserName
        {
            get { return Context.Headers[ServiceConstants.Server.UsernameHeader]; }
        }
    }
}
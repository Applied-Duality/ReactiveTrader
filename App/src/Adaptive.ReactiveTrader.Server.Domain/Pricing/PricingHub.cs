using System.Threading.Tasks;
using Adaptive.ReactiveTrader.Server.ReferenceData;
using Adaptive.ReactiveTrader.Server.Transport;
using Adaptive.ReactiveTrader.Shared;
using Adaptive.ReactiveTrader.Shared.Pricing;
using log4net;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace Adaptive.ReactiveTrader.Server.Pricing
{
    [HubName(ServiceConstants.Server.PricingHub)]
    public class PricingHub : Hub
    {
        private readonly IPriceLastValueCache _priceLastValueCache;
        private readonly ICurrencyPairRepository _currencyPairRepository;
        private readonly IContextHolder _contextHolder;
        public const string PriceStreamGroupPattern = "Pricing/{0}";

        private static readonly ILog Log = LogManager.GetLogger(typeof(PricingHub));

        public PricingHub(
            IPriceLastValueCache priceLastValueCache,
            ICurrencyPairRepository currencyPairRepository,
            IContextHolder contextHolder)
        {
            _priceLastValueCache = priceLastValueCache;
            _currencyPairRepository = currencyPairRepository;
            _contextHolder = contextHolder;
        }

        [HubMethodName(ServiceConstants.Server.SubscribePriceStream)]
        public async Task SubscribePriceStream(PriceSubscriptionRequestDto request)
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
        public async Task UnsubscribePriceStream(PriceSubscriptionRequestDto request)
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
    }
}
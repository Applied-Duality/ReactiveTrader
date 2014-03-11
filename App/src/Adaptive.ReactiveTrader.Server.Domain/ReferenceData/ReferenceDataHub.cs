using System.Collections.Generic;
using System.Linq;
using Adaptive.ReactiveTrader.Server.Transport;
using Adaptive.ReactiveTrader.Shared;
using Adaptive.ReactiveTrader.Shared.DTO;
using Adaptive.ReactiveTrader.Shared.DTO.ReferenceData;
using log4net;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace Adaptive.ReactiveTrader.Server.ReferenceData
{
    [HubName(ServiceConstants.Server.ReferenceDataHub)]

    public class ReferenceDataHub : Hub
    {
        private readonly ICurrencyPairRepository _currencyPairRepository;
        private readonly IContextHolder _contextHolder;
        private static readonly ILog Log = LogManager.GetLogger(typeof(ReferenceDataHub));

        public const string CurrencyPairUpdateGroupName = "CurrencyPairUpdateGroup";

        public ReferenceDataHub(ICurrencyPairRepository currencyPairRepository, IContextHolder contextHolder)
        {
            _currencyPairRepository = currencyPairRepository;
            _contextHolder = contextHolder;
        }

        [HubMethodName(ServiceConstants.Server.GetCurrencyPairs)]
        public IEnumerable<CurrencyPairUpdateDto> GetCurrencyPairs()
        {
            _contextHolder.ReferenceDataHubClients = Clients;
            Log.InfoFormat("Received request for currency pairs from connection {0}", Context.ConnectionId);

            var currencyPairs = _currencyPairRepository.GetAllCurrencyPairInfos().Where(cp=>cp.Enabled).Select(cp=>cp.CurrencyPair).ToList();
            Log.InfoFormat("Sending {0} currency pairs to {1}'", currencyPairs.Count, Context.ConnectionId);

            Groups.Add(Context.ConnectionId, CurrencyPairUpdateGroupName);

            return currencyPairs.Select(cp => new CurrencyPairUpdateDto
            {
                CurrencyPair = cp,
                UpdateType = UpdateTypeDto.Added
            });
        }
    }
}
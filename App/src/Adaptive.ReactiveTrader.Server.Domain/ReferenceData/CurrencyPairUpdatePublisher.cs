using System;
using System.Threading.Tasks;
using Adaptive.ReactiveTrader.Server.Transport;
using Adaptive.ReactiveTrader.Shared.ReferenceData;
using log4net;
using Microsoft.AspNet.SignalR.Hubs;

namespace Adaptive.ReactiveTrader.Server.ReferenceData
{
    public class CurrencyPairUpdatePublisher : ICurrencyPairUpdatePublisher
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (CurrencyPairUpdatePublisher));
        private readonly IContextHolder _contextHolder;

        public CurrencyPairUpdatePublisher(IContextHolder contextHolder)
        {
            _contextHolder = contextHolder;
        }

        public async Task Publish(CurrencyPairUpdateDto update)
        {
            IHubCallerConnectionContext context = _contextHolder.ReferenceDataHubClients;
            if (context == null) return;

            const string groupName = ReferenceDataHub.CurrencyPairUpdateGroupName;
            try
            {
                await context.Group(groupName).OnCurrencyPairUpdate(update);
                Log.InfoFormat("Published currency pair update to group {0}: {1}", groupName, update);
            }
            catch (Exception e)
            {
                Log.Error(
                    string.Format("An error occured while publishing currency pair update to group {0}: {1}", groupName,
                        update), e);
            }
        }
    }
}
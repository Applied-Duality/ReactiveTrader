using Adaptive.ReactiveTrader.Contracts;
using log4net;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace Adaptive.ReactiveTrader.Server.Execution
{
    [HubName(ServiceConstants.Server.ExecutionHub)]
    public class ExecutionHub : Hub
    {
        private readonly IExecutionService _executionService;
        private static readonly ILog Log = LogManager.GetLogger(typeof(ExecutionHub));

        public ExecutionHub(IExecutionService executionService)
        {
            _executionService = executionService;
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

        private string UserName
        {
            get { return Context.Headers[ServiceConstants.Server.UsernameHeader]; }
        }
    }
}
using System.Threading.Tasks;
using Adaptive.ReactiveTrader.Contracts;
using log4net;

namespace Adaptive.ReactiveTrader.Client.Transport
{
    public class ExecutionServiceClient : IExecutionServiceClient
    {
        private readonly ITransport _transport;
        private static readonly ILog Log = LogManager.GetLogger(typeof(SpotStreamRepository));

        public ExecutionServiceClient(ITransport transport)
        {
            _transport = transport;
        }

        public async Task<SpotTrade> Execute(SpotTradeRequest spotTradeRequest)
        {
            Log.InfoFormat("Sending trade request: {0}", spotTradeRequest);
            var trade = await _transport.ExecutionHubProxy.Invoke<SpotTrade>(ServiceConstants.Server.Execute, spotTradeRequest);

            Log.InfoFormat("Trade response received: {0}", trade);
            return trade;
        }
    }
}
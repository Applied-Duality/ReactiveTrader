using System;
using System.Reactive.Linq;
using Adaptive.ReactiveTrader.Client.Transport;
using Adaptive.ReactiveTrader.Contracts;
using Adaptive.ReactiveTrader.Contracts.Execution;
using Adaptive.ReactiveTrader.Contracts.Extensions;
using Microsoft.AspNet.SignalR.Client;

namespace Adaptive.ReactiveTrader.Client.ServiceClients.Execution
{
    class ExecutionServiceClient : IExecutionServiceClient
    {
        private readonly IHubProxy _executionHubProxy;

        public ExecutionServiceClient(ITransport transport)
        {
            _executionHubProxy = transport.GetProxy(ServiceConstants.Server.ExecutionHub);
        }

        public IObservable<SpotTrade> Execute(SpotTradeRequest spotTradeRequest)
        {
            return
                Observable.FromAsync(
                    () => _executionHubProxy.Invoke<SpotTrade>(ServiceConstants.Server.ExecutionHub, spotTradeRequest))
                    .CacheFirstResult();
        }
    }
}
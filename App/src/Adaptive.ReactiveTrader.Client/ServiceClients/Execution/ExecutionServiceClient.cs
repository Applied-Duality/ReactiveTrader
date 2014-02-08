using System;
using System.Reactive.Linq;
using Adaptive.ReactiveTrader.Client.Transport;
using Adaptive.ReactiveTrader.Shared;
using Adaptive.ReactiveTrader.Shared.Execution;
using Adaptive.ReactiveTrader.Shared.Extensions;
using Microsoft.AspNet.SignalR.Client;

namespace Adaptive.ReactiveTrader.Client.ServiceClients.Execution
{
    class ExecutionServiceClient : IExecutionServiceClient
    {
        private readonly IHubProxy _executionHubProxy;

        public ExecutionServiceClient(ISignalRTransport transport)
        {
            _executionHubProxy = transport.GetProxy(ServiceConstants.Server.ExecutionHub);
        }

        public IObservable<TradeDto> Execute(TradeRequestDto tradeRequest)
        {
            return
                Observable.FromAsync(
                    () => _executionHubProxy.Invoke<TradeDto>(ServiceConstants.Server.ExecutionHub, tradeRequest))
                    .CacheFirstResult();
        }
    }
}
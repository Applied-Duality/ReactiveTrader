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
        private readonly IConnectionProvider _connectionProvider;

        public ExecutionServiceClient(IConnectionProvider connectionProvider)
        {
            _connectionProvider = connectionProvider;
        }

        public IObservable<TradeDto> Execute(TradeRequestDto tradeRequest)
        {
            return (from connection in _connectionProvider.GetActiveConnection().Take(1) 
                from trade in ExecuteForConnection(connection.ExecutionHubProxy, tradeRequest)
                select trade)
                .CacheFirstResult();
        }

        private IObservable<TradeDto> ExecuteForConnection(IHubProxy executionHubProxy, TradeRequestDto tradeRequestDto)
        {
            return
                Observable.FromAsync(
                    () => executionHubProxy.Invoke<TradeDto>(ServiceConstants.Server.Execute, tradeRequestDto));
        } 
    }
}
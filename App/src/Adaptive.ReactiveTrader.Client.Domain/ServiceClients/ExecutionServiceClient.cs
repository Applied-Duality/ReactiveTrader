using System;
using System.Reactive.Linq;
using Adaptive.ReactiveTrader.Client.Domain.Transport;
using Adaptive.ReactiveTrader.Shared;
using Adaptive.ReactiveTrader.Shared.Execution;
using Microsoft.AspNet.SignalR.Client;

namespace Adaptive.ReactiveTrader.Client.Domain.ServiceClients
{
    class ExecutionServiceClient :ServiceClient, IExecutionServiceClient
    {
        public ExecutionServiceClient(IConnectionProvider connectionProvider) : base(connectionProvider)
        {
        }

        public IObservable<TradeDto> Execute(TradeRequestDto tradeRequest)
        {
            return RequestUponConnection(connection => ExecuteForConnection(connection.ExecutionHubProxy, tradeRequest), TimeSpan.FromMilliseconds(500));
        }

        private IObservable<TradeDto> ExecuteForConnection(IHubProxy executionHubProxy, TradeRequestDto tradeRequestDto)
        {
            return
                Observable.FromAsync(
                    () => executionHubProxy.Invoke<TradeDto>(ServiceConstants.Server.Execute, tradeRequestDto));
        } 
    }
}
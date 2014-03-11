using System;
using System.Reactive.Linq;
using Adaptive.ReactiveTrader.Client.Domain.Transport;
using Adaptive.ReactiveTrader.Shared;
using Adaptive.ReactiveTrader.Shared.DTO.Execution;
using Microsoft.AspNet.SignalR.Client;

namespace Adaptive.ReactiveTrader.Client.Domain.ServiceClients
{
    internal class ExecutionServiceClient : ServiceClientBase, IExecutionServiceClient
    {
        public ExecutionServiceClient(IConnectionProvider connectionProvider) : base(connectionProvider)
        {
        }

        public IObservable<TradeDto> Execute(TradeRequestDto tradeRequest)
        {
            return RequestUponConnection(connection => ExecuteForConnection(connection.ExecutionHubProxy, tradeRequest), TimeSpan.FromMilliseconds(500));
        }

        private static IObservable<TradeDto> ExecuteForConnection(IHubProxy executionHubProxy, TradeRequestDto tradeRequestDto)
        {
            return Observable.FromAsync(
                () => executionHubProxy.Invoke<TradeDto>(ServiceConstants.Server.Execute, tradeRequestDto));
        } 
    }
}

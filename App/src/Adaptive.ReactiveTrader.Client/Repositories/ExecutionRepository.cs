using System;
using System.Reactive.Linq;
using Adaptive.ReactiveTrader.Client.Models;
using Adaptive.ReactiveTrader.Client.ServiceClients.Execution;
using Adaptive.ReactiveTrader.Contracts.Execution;
using Adaptive.ReactiveTrader.Contracts.Extensions;
using Direction = Adaptive.ReactiveTrader.Contracts.Execution.Direction;

namespace Adaptive.ReactiveTrader.Client.Repositories
{
    class ExecutionRepository : IExecutionRepository
    {
        private readonly IExecutionServiceClient _executionServiceClient;
        private readonly ITradeFactory _tradeFactory;

        public ExecutionRepository(IExecutionServiceClient executionServiceClient, ITradeFactory tradeFactory)
        {
            _executionServiceClient = executionServiceClient;
            _tradeFactory = tradeFactory;
        }

        public IObservable<ITrade> Execute(IOneWayPrice oneWayPrice, long notional)
        {
            var request = new TradeRequest
            {
                Direction = Direction.Buy, // TODO
                Notional = notional,
                Price = null, // oneWayPrice.Parent.SpotPrice
            };

            return _executionServiceClient.Execute(request)
                .Select(_tradeFactory.Create)
                .CacheFirstResult();
        }
    }
}
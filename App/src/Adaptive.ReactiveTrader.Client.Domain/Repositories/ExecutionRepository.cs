using System;
using System.Reactive.Linq;
using Adaptive.ReactiveTrader.Client.Domain.Models;
using Adaptive.ReactiveTrader.Client.Domain.ServiceClients;
using Adaptive.ReactiveTrader.Shared.Execution;
using Adaptive.ReactiveTrader.Shared.Extensions;

namespace Adaptive.ReactiveTrader.Client.Domain.Repositories
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

        public IObservable<ITrade> Execute(IExecutablePrice executablePrice, long notional, string dealtCurrency)
        {
            var price = executablePrice.Parent;

            var request = new TradeRequestDto
            {
                Direction = executablePrice.Direction == Direction.BUY ? DirectionDto.Buy : DirectionDto.Sell,
                Notional = notional,
                QuoteId = price.QuoteId,
                SpotRate = executablePrice.Rate,
                Symbol = price.CurrencyPair.Symbol,
                ValueDate = price.ValueDate,
                DealtCurrency = dealtCurrency
            };

            return _executionServiceClient.Execute(request)
                .Timeout(TimeSpan.FromSeconds(2))
                .Select(_tradeFactory.Create)
                .CacheFirstResult();
        }
    }
}
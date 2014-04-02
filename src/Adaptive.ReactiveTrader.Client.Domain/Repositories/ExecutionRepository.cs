using System;
using System.Reactive.Linq;
using Adaptive.ReactiveTrader.Client.Domain.Concurrency;
using Adaptive.ReactiveTrader.Client.Domain.Models;
using Adaptive.ReactiveTrader.Client.Domain.Models.Execution;
using Adaptive.ReactiveTrader.Client.Domain.Models.Pricing;
using Adaptive.ReactiveTrader.Client.Domain.ServiceClients;
using Adaptive.ReactiveTrader.Shared.DTO.Execution;
using Adaptive.ReactiveTrader.Shared.Extensions;

namespace Adaptive.ReactiveTrader.Client.Domain.Repositories
{
    class ExecutionRepository : IExecutionRepository
    {
        private readonly IExecutionServiceClient _executionServiceClient;
        private readonly ITradeFactory _tradeFactory;
        private readonly IConcurrencyService _concurrencyService;

        public ExecutionRepository(IExecutionServiceClient executionServiceClient, ITradeFactory tradeFactory, IConcurrencyService concurrencyService)
        {
            _executionServiceClient = executionServiceClient;
            _tradeFactory = tradeFactory;
            _concurrencyService = concurrencyService;
        }

        public IObservable<IStale<ITrade>> ExecuteRequest(IExecutablePrice executablePrice, long notional, string dealtCurrency)
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

            return _executionServiceClient.ExecuteRequest(request)
                .Select(_tradeFactory.Create)
                .DetectStale(TimeSpan.FromSeconds(2), _concurrencyService.ThreadPool);
        }
    }
}
using System;
using System.Threading;
using Adaptive.ReactiveTrader.Server.Blotter;
using Adaptive.ReactiveTrader.Shared.Execution;

namespace Adaptive.ReactiveTrader.Server.Execution
{
    class ExecutionService : IExecutionService
    {
        private readonly IBlotterPublisher _blotterPublisher;
        private readonly ITradeRepository _tradeRepository;
        private readonly Random _random;
        private long _tradeId;

        public ExecutionService(IBlotterPublisher blotterPublisher, ITradeRepository tradeRepository)
        {
            _blotterPublisher = blotterPublisher;
            _tradeRepository = tradeRepository;
            _random = new Random();
            _tradeId = 0;
        }

        public TradeDto Execute(TradeRequestDto tradeRequest, string user)
        {
            var trade =  new TradeDto
            {
                CurrencyPair = tradeRequest.Symbol,
                Direction = tradeRequest.Direction,
                Notional = tradeRequest.Notional,
                SpotRate = tradeRequest.SpotRate,
                Status = _random.Next(0, 5) > 3 ? TradeStatusDto.Rejected : TradeStatusDto.Done,
                TradeDate = DateTime.UtcNow,
                ValueDate = tradeRequest.ValueDate,
                TradeId = Interlocked.Increment(ref _tradeId),
                TraderName = user
            };

            _tradeRepository.StoreTrade(trade);

            // publish trade asynchronously
            _blotterPublisher.Publish(trade);

            return trade;
        }
    }
}
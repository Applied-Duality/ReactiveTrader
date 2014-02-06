using System;
using System.Threading;
using Adaptive.ReactiveTrader.Contracts;

namespace Adaptive.ReactiveTrader.Server
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

        public SpotTrade Execute(SpotTradeRequest tradeRequest, string user)
        {
            var trade =  new SpotTrade
            {
                CurrencyPair = tradeRequest.Price.Symbol,
                Direction = tradeRequest.Direction,
                Notional = tradeRequest.Notional,
                SpotPrice = tradeRequest.Direction == Direction.Buy ? tradeRequest.Price.Ask : tradeRequest.Price.Bid,
                Status = _random.Next(0, 5) > 3 ? TradeStatus.Rejected : TradeStatus.Done,
                TradeDate = DateTime.UtcNow.Date,
                ValueDate = tradeRequest.Price.ValueDate,
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
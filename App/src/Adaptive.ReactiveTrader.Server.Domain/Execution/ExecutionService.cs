using System;
using System.Threading;
using System.Threading.Tasks;
using Adaptive.ReactiveTrader.Server.Blotter;
using Adaptive.ReactiveTrader.Shared.Execution;

namespace Adaptive.ReactiveTrader.Server.Execution
{
    public class ExecutionService : IExecutionService
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

        public async Task<TradeDto> Execute(TradeRequestDto tradeRequest, string user)
        {
            if (tradeRequest.Symbol == "EURJPY")
            {
                await Task.Delay(TimeSpan.FromSeconds(5));
            }

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
using System;
using System.Threading;
using System.Threading.Tasks;
using Adaptive.ReactiveTrader.Server.Blotter;
using Adaptive.ReactiveTrader.Shared.DTO.Execution;

namespace Adaptive.ReactiveTrader.Server.Execution
{
    public class ExecutionService : IExecutionService
    {
        private readonly IBlotterPublisher _blotterPublisher;
        private readonly ITradeRepository _tradeRepository;
        private long _tradeId;

        public ExecutionService(IBlotterPublisher blotterPublisher, ITradeRepository tradeRepository)
        {
            _blotterPublisher = blotterPublisher;
            _tradeRepository = tradeRepository;
            _tradeId = 0;
        }

        public async Task<TradeDto> Execute(TradeRequestDto tradeRequest, string user)
        {
            var status = TradeStatusDto.Done;

            switch (tradeRequest.Symbol)
            {
                case "EURJPY":
                    await Task.Delay(TimeSpan.FromSeconds(5));
                    break;
                case "GBPUSD":
                    await Task.Delay(TimeSpan.FromSeconds(1.5));
                    break;
                default:
                    await Task.Delay(TimeSpan.FromSeconds(.5));
                    break;
            }
           
            if (tradeRequest.Symbol == "GBPJPY")
            {
                status = TradeStatusDto.Rejected;
            }

            var trade =  new TradeDto
            {
                CurrencyPair = tradeRequest.Symbol,
                Direction = tradeRequest.Direction,
                Notional = tradeRequest.Notional,
                SpotRate = tradeRequest.SpotRate,
                Status = status,
                TradeDate = DateTime.UtcNow,
                ValueDate = tradeRequest.ValueDate,
                TradeId = Interlocked.Increment(ref _tradeId),
                TraderName = user,
                DealtCurrency = tradeRequest.DealtCurrency
            };

            _tradeRepository.StoreTrade(trade);

            // publish trade asynchronously
            await _blotterPublisher.Publish(trade);

            return trade;
        }
    }
}
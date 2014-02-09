using Adaptive.ReactiveTrader.Shared.Execution;

namespace Adaptive.ReactiveTrader.Client.Models
{
    class TradeFactory : ITradeFactory
    {
        public ITrade Create(TradeDto trade)
        {
            return new Trade(
                trade.CurrencyPair,
                trade.Direction == DirectionDto.Buy ? Direction.Buy : Direction.Sell,
                trade.Notional,
                trade.SpotRate,
                trade.Status == TradeStatusDto.Done ? TradeStatus.Done : TradeStatus.Rejected,
                trade.TradeDate,
                trade.TradeId,
                trade.TraderName,
                trade.ValueDate);
        }
    }
}
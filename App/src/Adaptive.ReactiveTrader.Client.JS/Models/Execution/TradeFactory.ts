class TradeFactory implements ITradeFactory {
    create(trade: TradeDto) {
        return new Trade(
                trade.CurrencyPair,
                trade.Direction == DirectionDto.Buy ? Direction.Buy : Direction.Sell,
                trade.Notional,
                trade.SpotRate,
                trade.Status == TradeStatusDto.Done ? TradeStatus.Done : TradeStatus.Rejected,
                trade.TradeDate,
                trade.TradeId,
                trade.TraderName,
                trade.ValueDate,
                trade.DealtCurrency);
    }
} 






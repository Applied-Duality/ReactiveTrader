class TradeDto {
    TradeId:number;
    TraderName: string;
    CurrencyPair: string;
    Notional:number;
    DealtCurrency: string;
    Direction: DirectionDto;
    SpotRate: number;
    TradeDate: Date;
    ValueDate:Date;
    Status:TradeStatus;
}

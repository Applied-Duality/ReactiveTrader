interface ITrade {
    currencyPair: string;
    direction: Direction;
    notional: number;
    spotRate: number;
    tradeStatus: TradeStatus;
    tradeDate: Date;
    tradeId: number;
    traderName:string;
    valueDate:Date;
    dealtCurrency: string;
} 
 class Trade implements ITrade {
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
    
    constructor(
        currencyPair: string,
        direction: Direction,
        notional: number,
        spotRate: number,
        tradeStatus: TradeStatus,
        tradeDate: Date,
        tradeId: number,
        traderName:string,
        valueDate:Date,
        dealtCurrency: string){
    
        this.currencyPair = currencyPair;
        this.direction = direction;
        this.notional = notional;
        this.spotRate = spotRate;
        this.tradeStatu = tradeStatus;
        this.tradeDate = tradeDate;
        this.tradeId = tradeId;
        this.traderName = traderName;
        this.valueDate = valueDate;
        this.dealtCurrency = dealtCurrency;
    }    
}
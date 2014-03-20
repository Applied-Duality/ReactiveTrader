class Price implements IPrice {
    constructor(
        bid: ExecutablePrice,
        ask: ExecutablePrice,
        mid: number,
        quoteId: number,
        valueDate: Date,
        currencyPair: ICurrencyPair) {

        this.bid = bid;
        this.ask = ask;
        this.mid = mid;
        this.quoteId = quoteId;
        this.valueDate = valueDate;
        this.currencyPair = currencyPair;
        this.isStale = false;

        bid.parent = this;
        ask.parent = this;

        this.spread = (ask.rate - bid.rate) * Math.pow(10, currencyPair.pipsPosition);
    }

    // TODO encapsulate as fields
    bid: IExecutablePrice;
    ask: IExecutablePrice;
    mid: number; 
    currencyPair: ICurrencyPair;
    quoteId: number;
    valueDate: Date;
    spread: number;
    isStale: boolean; 
} 
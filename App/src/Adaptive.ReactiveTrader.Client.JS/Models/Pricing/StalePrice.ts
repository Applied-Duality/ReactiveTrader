class StalePrice implements IPrice {
    // TODO encapsulate and throw for properpties that should not be accessed (see C# code)
    bid: IExecutablePrice;
    ask: IExecutablePrice;
    mid: number; 
    currencyPair: ICurrencyPair;
    quoteId: number;
    valueDate: Date;
    spread: number;
    isStale: boolean; 

    constructor(currencyPair: ICurrencyPair) {
        this.currencyPair = currencyPair;
        this.isStale = true;
    }
}  
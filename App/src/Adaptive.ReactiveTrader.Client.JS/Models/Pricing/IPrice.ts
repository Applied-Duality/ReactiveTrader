interface IPrice
{
    bid: IExecutablePrice;
    ask: IExecutablePrice;
    mid: number; 
    currencyPair: ICurrencyPair;
    quoteId: number;
    valueDate: Date;
    spread: number;
    isStale: boolean; 
}


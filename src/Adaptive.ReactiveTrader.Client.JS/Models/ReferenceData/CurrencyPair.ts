class CurrencyPair implements ICurrencyPair {
    symbol: string;
    prices: Rx.Observable<IPrice>;
    ratePrecision: number;
    pipsPosition: number;
    baseCurrency: string;
    counterCurrency: string;

    constructor(symbol: string, ratePrecision: number, pipsPosition: number, priceRespository: IPriceRepository) {
        this.symbol = symbol;
        this.ratePrecision = ratePrecision;
        this.pipsPosition = pipsPosition;
        this.baseCurrency = symbol.substring(0, 3);
        this.counterCurrency = symbol.substring(3, 6);

        this.prices =
            Rx.Observable.defer<IPrice>(()=> priceRespository.getPrices(<ICurrencyPair>this))
            .publish()
            .refCount();
    }
} 
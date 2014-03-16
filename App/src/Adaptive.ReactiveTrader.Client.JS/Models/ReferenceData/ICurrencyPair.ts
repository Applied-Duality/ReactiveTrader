interface ICurrencyPair {
    symbol: string;
    prices: Rx.Observable<IPrice>;
    ratePrecision: number;
    pipsPosition: number;
    baseCurrency: string;
    counterCurrency: string;
} 
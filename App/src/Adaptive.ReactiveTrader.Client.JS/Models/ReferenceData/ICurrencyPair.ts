interface ICurrencyPair {
    Symbol: string;
    Prices: Rx.Observable<IPrice>;
    RatePrecision: number;
    PipsPosition: number;
    BaseCurrency: string;
    CounterCurrency: string;
} 
interface IPriceRepository {
    getPrices(currencyPair: ICurrencyPair) : Rx.Observable<IPrice>;
} 
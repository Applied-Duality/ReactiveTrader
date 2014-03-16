class CurrencyPair implements ICurrencyPair {
    private _symbol: string;
    private _ratePrecision: number;
    private _pipsPosition: number;
    private _baseCurrency: string;
    private _counterCurrency: string;
    private _prices: Rx.Observable<IPrice>

    constructor(symbol: string, ratePrecision: number, pipsPosition: number, priceRespository: IPriceRepository) {
        this._symbol = symbol;
        this._ratePrecision = ratePrecision;
        this._pipsPosition = pipsPosition;
        this._baseCurrency = symbol.substring(0, 3);
        this._counterCurrency = symbol.substring(3, 6);

        this._prices =
            Rx.Observable.defer<IPrice>(()=> priceRespository.getPrices(this))
            .publish()
            .refCount();
    }

    public get Symbol() {
        return this._symbol;
    }

    public get RatePrecision() {
        return this._ratePrecision;
    }

    public get PipsPositio() {
        return this._pipsPosition;
    }

    public get BaseCurrency() {
        return this._baseCurrency;
    }

    public get CounterCurrency() {
        return this._counterCurrency;
    }

    public get Prices() {
        return this._prices;
    }
} 
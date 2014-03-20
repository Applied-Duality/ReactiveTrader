class CurrencyPairUpdate implements ICurrencyPairUpdate {
    private _updateType: UpdateType;
    private _currencyPair: ICurrencyPair;

    constructor(updateType: UpdateType, currencyPair: ICurrencyPair) {
        this._currencyPair = currencyPair;
        this._updateType = updateType;
    }

    public get updateType() {
        return this._updateType;
    }

    public get currencyPair() {
        return this._currencyPair;
    }
} 
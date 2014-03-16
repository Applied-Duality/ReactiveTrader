class CurrencyPairUpdateFactory implements ICurrencyPairUpdateFactory {
    private _priceRepository: IPriceRepository;

    constructor(priceRepository: IPriceRepository) {
        this._priceRepository = priceRepository;
    }

    create(currencyPairUpdate: CurrencyPairUpdateDto): ICurrencyPairUpdate {
        var cp = new CurrencyPair(
            currencyPairUpdate.CurrencyPair.Symbol,
            currencyPairUpdate.CurrencyPair.RatePrecision,
            currencyPairUpdate.CurrencyPair.PipsPrecision,
            this._priceRepository);

        var update = new CurrencyPairUpdate(
            currencyPairUpdate.UpdateType == UpdateTypeDto.Added ? UpdateType.Add : UpdateType.Remove,
            <ICurrencyPair>cp);

        return update;
    }
}
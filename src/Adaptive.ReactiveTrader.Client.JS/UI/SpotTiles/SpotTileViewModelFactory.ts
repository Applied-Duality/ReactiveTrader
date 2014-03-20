class SpotTileViewModelFactory implements ISpotTileViewModelFactory {
    private _priceLatencyRecorder: IPriceLatencyRecorder;
    
    constructor(priceLatencyRecorder: IPriceLatencyRecorder) {
        this._priceLatencyRecorder = priceLatencyRecorder;
    }

    create(currencyPair: ICurrencyPair): ISpotTileViewModel {
        return new SpotTileViewModel(currencyPair, this._priceLatencyRecorder);
    }
}
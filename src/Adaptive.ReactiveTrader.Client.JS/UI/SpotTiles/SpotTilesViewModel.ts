class SpotTilesViewModel implements ISpotTilesViewModel {
    spotTiles: KnockoutObservableArray<ISpotTileViewModel>;
    private _referenceDataRepository: IReferenceDataRepository;
    private _spotTileViewModelFactory: ISpotTileViewModelFactory;

    constructor(referenceDataRepository: IReferenceDataRepository, spotTileViewModelFactory: ISpotTileViewModelFactory) {
        this._referenceDataRepository = referenceDataRepository;
        this._spotTileViewModelFactory = spotTileViewModelFactory;
        this.spotTiles = ko.observableArray([]);

        this.loadSpotTiles();
    }

    private loadSpotTiles(): void {
        this._referenceDataRepository.getCurrencyPairs()
            .subscribe(
                currencyPairs=> currencyPairs.forEach(cp=> this.handleCurrencyPairUpdate(cp)),
                ex=> console.error("Failed to get currencies", ex));
    }

    private handleCurrencyPairUpdate(update: ICurrencyPairUpdate) {
        var spotTileViewModel = ko.utils.arrayFirst(this.spotTiles(), stvm=> stvm.symbol == update.currencyPair.symbol);
        if (update.updateType == UpdateType.Add) {
            if (spotTileViewModel != null) {
                // we already have a tile for this ccy pair
                return;
            }

            var spotTile = this._spotTileViewModelFactory.create(update.currencyPair);
            this.spotTiles.push(spotTile);
        } else {
            if (spotTileViewModel != null) {
                this.spotTiles.remove(spotTileViewModel);
                spotTileViewModel.dispose();
            }                
        }
    }
} 
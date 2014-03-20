class AppViewModel {
    spotTiles: KnockoutObservableArray<SpotTileViewModel>;
    trades: KnockoutObservableArray<ITrade>;

    constructor(reactiveTrader: IReactiveTrader) {
        this.spotTiles = ko.observableArray(<SpotTileViewModel[]>[]);
        this.trades = ko.observableArray(<ITrade[]>[]);

        reactiveTrader.referenceDataRepository.getCurrencyPairs()
            .subscribe(currencyPairUpdates=> {
                for (var i = 0; i < currencyPairUpdates.length; i++) {
                    var update = currencyPairUpdates[i];
                    if (update.updateType == UpdateType.Add) {
                        this.spotTiles.push(new SpotTileViewModel(update.currencyPair));
                    }
                }
            });

        reactiveTrader.tradeRepository.getTrades()
            .subscribe(ts=> {
                for (var i = 0; i < ts.length; i++) {
                    this.trades.push(ts[i]);
                }
            },
            ex => console.error(ex));
    }
} 
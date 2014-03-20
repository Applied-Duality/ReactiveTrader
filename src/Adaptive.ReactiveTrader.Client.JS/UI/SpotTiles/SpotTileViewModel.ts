class SpotTileViewModel implements ISpotTileViewModel {
    private _priceSubscription: Rx.SerialDisposable;
    private _currencyPair: ICurrencyPair;
    private _previousRate: number;
    private _priceLatencyRecorder: IPriceLatencyRecorder;

    constructor(currencyPair: ICurrencyPair, priceLatencyRecorder: IPriceLatencyRecorder) {
        this._priceLatencyRecorder = priceLatencyRecorder;
        this.symbol = currencyPair.baseCurrency + " / " + currencyPair.counterCurrency;
        this._priceSubscription = new Rx.SerialDisposable();
        this._currencyPair = currencyPair;
        this.bid = new OneWayPriceViewModel(this, Direction.Sell);
        this.ask = new OneWayPriceViewModel(this, Direction.Buy);
        this.notional = ko.observable(1000000);
        this.dealtCurrency = currencyPair.baseCurrency;
        this.spread = ko.observable("");
        this.movement = ko.observable(PriceMovement.None);
        this.spotDate = ko.observable("SP");
        this.isSubscribing = ko.observable(true);

        this.subscribeForPrices();
    }

    symbol: string;
    bid: IOneWayPriceViewModel;
    ask: IOneWayPriceViewModel;
    notional: KnockoutObservable<number>;
    spread: KnockoutObservable<string>;
    dealtCurrency: string;
    movement: KnockoutObservable<PriceMovement>;
    spotDate: KnockoutObservable<string>;
    isSubscribing: KnockoutObservable<boolean>;

    private _disposed: boolean;

    dispose(): void {
        if (!this._disposed) {
            this._priceSubscription.dispose();
            this._disposed = true;
        }
    }

    executeBid() {
        this.bid.onExecute();
    }

    executeAsk() {
        this.ask.onExecute();
    }

    onTrade(trade: ITrade): void {
        // TODO
    }

    private subscribeForPrices(): void {
        var subscription = this._currencyPair.prices
            .subscribe(
                price => this.onPrice(price),
                ex => console.error(ex));

        this._priceSubscription.setDisposable(subscription);
    }

    private onPrice(price: IPrice): void {
        this.isSubscribing(false);

        if (price.isStale) {
            this.bid.onStalePrice();
            this.ask.onStalePrice();
            this.spread("");
            this._previousRate = null;
            this.movement(PriceMovement.None);
            this.spotDate("SP");
        } else {
            if (this._previousRate != null) {
                if (price.mid > this._previousRate) {
                    this.movement(PriceMovement.Up);
                } 
                else if (price.mid < this._previousRate) {
                    this.movement(PriceMovement.Down);
                } 
                else {
                    this.movement(PriceMovement.None);
                }
            }

            this._previousRate = price.mid;
            this.bid.onPrice(price.bid);
            this.ask.onPrice(price.ask);

            this.spread(PriceFormatter.getFormattedSpread(price.spread, this._currencyPair.ratePrecision, this._currencyPair.pipsPosition));
            this.spotDate("SP."); //TODO

            this._priceLatencyRecorder.record(price);
        }
    }
} 




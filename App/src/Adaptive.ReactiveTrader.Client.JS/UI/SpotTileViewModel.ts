class SpotTileViewModel {
    private _priceSubscription: Rx.Disposable;
    private _price: IPrice;
    private _currencyPair: ICurrencyPair;

    constructor(currencyPair: ICurrencyPair) {
        this.symbol = currencyPair.symbol;
        this.bid = ko.observable(0);
        this.ask = ko.observable(0);
        this._currencyPair = currencyPair;

        this._priceSubscription = currencyPair.prices.subscribe(
            price=> {
                this._price = price;
                this.bid(price.bid.rate);
                this.ask(price.ask.rate);
            });
    }

    executeBid() {
        if (this._price == null) return;

        this._price.bid.execute(1000000, this._currencyPair.baseCurrency)
            .subscribe(
                trade=> console.log("Trade " + (trade.tradeStatus == TradeStatus.Done ? "done!" : "rejected!")),
                ex=> console.error(ex));
    }

    executeAsk() {
        if (this._price == null) return;
        
        this._price.ask.execute(1000000, this._currencyPair.baseCurrency)
            .subscribe(
                trade=> console.log("Trade " + (trade.tradeStatus == TradeStatus.Done ? "done!" : "rejected!")),
                ex=> console.error(ex));
    }

    symbol: string;
    bid: KnockoutObservable<number>;
    ask: KnockoutObservable<number>;
} 
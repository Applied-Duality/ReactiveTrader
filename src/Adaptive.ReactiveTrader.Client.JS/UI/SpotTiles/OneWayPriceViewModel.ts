class OneWayPriceViewModel implements IOneWayPriceViewModel {
    private _parent: ISpotTileViewModel;
    private _executablePrice: IExecutablePrice;

    constructor(parent: ISpotTileViewModel, direction: Direction) {
        this._parent = parent;
        this.direction = direction == Direction.Buy ? "BUY" : "SELL";

        this.bigFigures = ko.observable("");
        this.pips = ko.observable("");
        this.tenthOfPips = ko.observable("");
        this.isExecuting = ko.observable(false);
        this.isStale = ko.observable(true);
    }

    onExecute(): void {
        this.isExecuting(true);

        this._executablePrice.execute(this._parent.notional(), this._parent.dealtCurrency)
            .subscribe(
                trade => this.onExecuted(trade),
                ex => this.onExecutionError(ex));
    }

    onPrice(executablePrice: IExecutablePrice): void {
        this._executablePrice = executablePrice;

        var formattedPrice = PriceFormatter.getFormattedPrice(executablePrice.rate,
            executablePrice.parent.currencyPair.ratePrecision, executablePrice.parent.currencyPair.pipsPosition);

        this.bigFigures(formattedPrice.bigFigures);
        this.pips(formattedPrice.pips);
        this.tenthOfPips(formattedPrice.tenthOfPips);
        this.isStale(false);
    }

    onStalePrice(): void {
        this._executablePrice = null;
        this.bigFigures("");
        this.pips("");
        this.tenthOfPips("");
        this.isStale(true);
    }

    private onExecuted(trade: ITrade) {
        console.log("Trade executed.");
        this._parent.onTrade(trade);
        this.isExecuting(true);
    }

    private onExecutionError(ex: Error) {
        // TODO
        console.error(ex);
    }

    direction: string;
    bigFigures: KnockoutObservable<string>;
    pips: KnockoutObservable<string>;
    tenthOfPips: KnockoutObservable<string>;
    isExecuting: KnockoutObservable<boolean>;
    isStale: KnockoutObservable<boolean>;
} 
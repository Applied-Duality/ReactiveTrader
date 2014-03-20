class BlotterViewModel implements IBlotterViewModel {
    private _tradeRepository : ITradeRepository;
    private _stale: boolean;

    trades: KnockoutObservableArray<ITradeViewModel>;

    constructor(tradeRepository: ITradeRepository) {
        this._tradeRepository = tradeRepository;
        this.trades = ko.observableArray([]);

        this.loadTrades();
    }

    private loadTrades(): void {
        this._tradeRepository.getTrades()
            .subscribe(
                trades => this.addTrades(trades),
                ex=> console.error("an error occured within the trade stream", ex));
    }

    private addTrades(trades: ITrade[]): void {
        if (trades.length == 0)
        {
            // empty list of trades means we are disconnected
            this._stale = true;
        }
        else
        {
            if (this._stale) {
                this.trades.removeAll();
                this._stale = false;
            }
        }

        trades.forEach(t=> {
            var tradeViewModel = new TradeViewModel(t);
            this.trades.push(tradeViewModel);
        });
    }
} 
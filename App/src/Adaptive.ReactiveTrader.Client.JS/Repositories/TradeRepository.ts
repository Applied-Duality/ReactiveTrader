class TradeRepository implements ITradeRepository {
    private _blotterServiceClient : IBlotterServiceClient;
    private _tradeFactory : ITradeFactory;

    constructor(
        blotterServiceClient: IBlotterServiceClient,
        tradeFactory: ITradeFactory) {
        this._tradeFactory = tradeFactory;
        this._blotterServiceClient = blotterServiceClient;
    }

    public getTrades() {
        return Rx.Observable.defer(()=> this._blotterServiceClient.getTradesStream())
            .select(trades=> this.createTrades(trades))
            .catch(()=> Rx.Observable.return([]))
            .repeat()
            .publish()
            .refCount();
    }

    private createTrades(trades: TradeDto[]): ITrade[] {
         var result = [];

        for (var i = 0; i < trades.length; i++) {
            result[i] = this._tradeFactory.create(trades[i]);
        }

        return result;
    }
} 
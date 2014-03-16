interface ITradeRepository {
    getTrades() : Rx.Observable<ITrade[]>;
}
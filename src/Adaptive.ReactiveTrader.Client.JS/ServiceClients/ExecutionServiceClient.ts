class ExecutionServiceClient implements IExecutionServiceClient {
    private _connection: IConnection;

    constructor(connection: IConnection) {
        this._connection = connection;
    }

    public execute(tradeRequest: TradeRequestDto): Rx.Observable<TradeDto> {
        return this.executeForConnection(tradeRequest, this._connection.executionHubProxy);
    }

    public executeForConnection(tradeRequest: TradeRequestDto, executionHub: HubProxy): Rx.Observable<TradeDto> {
        return Rx.Observable.create<TradeDto>(observer=> {
            executionHub.invoke("Execute", tradeRequest)
                .done(trade=> observer.onNext(trade))
                .fail(error=> observer.onError(error));

            return Rx.Disposable.empty;
        })
        .take(1)
        .timeout(2000);
        // TODO cachefirstresult
    }
}
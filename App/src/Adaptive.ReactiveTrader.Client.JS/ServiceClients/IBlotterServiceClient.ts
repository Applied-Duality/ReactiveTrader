interface IBlotterServiceClient {
    getTradesStream(): Rx.Observable<TradeDto[]>
}
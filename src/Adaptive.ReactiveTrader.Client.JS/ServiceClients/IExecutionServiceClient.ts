interface IExecutionServiceClient {
    execute(tradeRequest: TradeRequestDto): Rx.Observable<TradeDto>;
} 
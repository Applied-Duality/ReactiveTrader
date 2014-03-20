class ExecutionRepository implements IExecutionRepository {
    private _executionServiceClient : IExecutionServiceClient;
    private _tradeFactory: ITradeFactory ;

    constructor(
        executionServiceClient: IExecutionServiceClient,
        tradeFactory: ITradeFactory) {
        this._tradeFactory = tradeFactory;
        this._executionServiceClient = executionServiceClient;
    }

    execute(executablePrice: IExecutablePrice, notional: number, dealtCurrency: string) {
        var price = executablePrice.parent;

        var request = new TradeRequestDto();
        request.Direction = executablePrice.direction == Direction.Buy ? DirectionDto.Buy : DirectionDto.Sell;
        request.Notional = notional;
        request.QuoteId = price.quoteId;
        request.SpotRate = executablePrice.rate;
        request.Symbol = price.currencyPair.symbol;
        request.ValueDate = price.valueDate;
        request.DealtCurrency = dealtCurrency;

        return this._executionServiceClient.execute(request)
            .timeout(2000)
            .select(tradeDto=> this._tradeFactory.create(tradeDto))
            .take(1);
            // TODO .CacheFirstResult();
    }
} 
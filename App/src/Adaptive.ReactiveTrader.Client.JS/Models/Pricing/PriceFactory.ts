class PriceFactory implements IPriceFactory {
    private _executionRepository: IExecutionRepository;

    constructor(executionRepository: IExecutionRepository) {
        this._executionRepository = executionRepository;
    }

    create(priceDto: PriceDto, currencyPair: ICurrencyPair) {
        var bid = new ExecutablePrice(Direction.Sell, priceDto.Bid, this._executionRepository);
        var ask = new ExecutablePrice(Direction.Buy, priceDto.Ask, this._executionRepository);
        var price = new Price(bid, ask, priceDto.Mid, priceDto.QuoteId, priceDto.ValueDate, currencyPair);

        return price;
    }
} 


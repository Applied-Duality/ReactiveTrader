var ReactiveTrader = (function () {
    function ReactiveTrader() {
    }
    ReactiveTrader.prototype.initialize = function (username, server) {
        this._connection = new Connection(server, username);

        var referenceDataServiceClient = new ReferenceDataServiceClient(this._connection);
        var executionServiceClient = new ExecutionServiceClient(this._connection);
        var blotterServiceClient = new BlotterServiceClient(this._connection);
        var pricingServiceClient = new PricingServiceClient(this._connection);

        var tradeFactory = new TradeFactory();
        var executionRepository = new ExecutionRepository(executionServiceClient, tradeFactory);
        var priceFactory = new PriceFactory(executionRepository);
        var priceRepository = new PriceRepository(pricingServiceClient, priceFactory);
        var currencyPairUpdateFactory = new CurrencyPairUpdateFactory(priceRepository);

        this.tradeRepository = new TradeRepository(blotterServiceClient, tradeFactory);
        this.referenceDataRepository = new ReferenceDataRepository(referenceDataServiceClient, currencyPairUpdateFactory);

        return this._connection.initialize();
    };

    Object.defineProperty(ReactiveTrader.prototype, "connectionStatusStream", {
        get: function () {
            return this._connection.status.publish().refCount();
        },
        enumerable: true,
        configurable: true
    });
    return ReactiveTrader;
})();

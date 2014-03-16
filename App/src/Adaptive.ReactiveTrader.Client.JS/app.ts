window.onload = () => {
    var el = document.getElementById('content');

    var reactiveTrader: IReactiveTrader = new ReactiveTrader();

    reactiveTrader
        .initialize("olivier", "http://localhost:800")
        .subscribe(_=> {
            reactiveTrader.connectionStatusStream.subscribe(s=> console.log("Connection status: " + s));

            reactiveTrader.referenceDataRepository.getCurrencyPairs()
                .select(currencyPairs=> currencyPairs[0])
                .selectMany(currencyPairUpdate=> currencyPairUpdate.currencyPair.prices)
                .select((price: IPrice)=> price.bid.rate + " / " + price.ask.rate)
                .subscribe(p=> console.log(p));
        },
            ex=> console.error(ex));


    //var connection = new Connection("http://localhost:8080", "Olivier");

    //connection.status
    //    .subscribe(
    //        status => console.log("Connection status changed to " + status),
    //        ex => console.log(ex));

    //connection
    //    .initialize()
    //    .subscribe(
    //        _=> {
    //            console.log("Connected");
    //            var refData = new ReferenceDataServiceClient(connection);
    //            refData.getCurrencyPairUpdates()
    //                .subscribe(
    //                    currencyPairs=> console.log(currencyPairs),
    //                    ex=> console.error(ex));

    //            var pricing = new PricingServiceClient(connection);
    //            pricing.getSpotStream("EURUSD")
    //                .subscribe(
    //                    (price: PriceDto)=> console.log(price.Bid + "/" + price.Ask),
    //                    ex=> console.error(ex));
    //        },
    //        ex=> console.log(ex));
};
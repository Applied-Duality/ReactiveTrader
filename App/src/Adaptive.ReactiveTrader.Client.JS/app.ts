/// <reference path="typings/signalr/signalr.d.ts" />
/// <reference path="typings/rx.js/rx.d.ts"/>
/// <reference path="Transport/Connection.ts" />
/// <reference path="ServiceClients/ReferenceDataServiceClient.ts" />

window.onload = () => {
    var el = document.getElementById('content');

    
    var connection = new Connection("http://localhost:8080", "Olivier");

    connection.status
        .subscribe(
            status => console.log("Connection status changed to " + status),
            ex => console.log(ex));

    connection
        .initialize()
        .subscribe(
            _=> {
                console.log("Connected");
                var refData = new ReferenceDataServiceClient(connection);
                refData.getCurrencyPairUpdates()
                    .subscribe(
                        currencyPairs=> console.log(currencyPairs),
                        ex=> console.error(ex));

                var pricing = new PricingServiceClient(connection);
                pricing.getSpotStream("EURUSD")
                    .subscribe(
                        (price: IPriceDto)=> console.log(price.Bid + "/" + price.Ask),
                        ex=> console.error(ex));
            },
            ex=> console.log(ex));
};
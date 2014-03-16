/// <reference path="../typings/rx.js/rx.d.ts"/>
/// <reference path="../typings/signalr/signalr.d.ts"/>

class ReferenceDataServiceClient implements IReferenceDataServiceClient
{
    private _connection: IConnection;

    constructor(connection: IConnection){
        this._connection = connection;
    }

    getCurrencyPairUpdates() : Rx.Observable<CurrencyPairUpdateDto[]> {
        return this.getTradesForConnection(this._connection.referenceDataHubProxy);
    }

    private getTradesForConnection(referenceDataHubProxy: HubProxy) : Rx.Observable<CurrencyPairUpdateDto[]> {
        return Rx.Observable.create<CurrencyPairUpdateDto[]>(observer => {
            var currencyPairUpdateSubscription = this._connection.currencyPairUpdates.subscribe(
                currencyPairUpdate=> observer.onNext([currencyPairUpdate]));

            console.log("Sending currency pair subscription...");

            referenceDataHubProxy
                .invoke("GetCurrencyPairs")
                .done(currencyPairs => {
                    observer.onNext(currencyPairs);
                    console.log("Subscribed to currency pairs and received " + currencyPairs.length +" currency pairs.");
                })
                .fail(ex => observer.onError(ex));

            return currencyPairUpdateSubscription;
        });
        //.publish()
        //.refCount();
    }
}

        

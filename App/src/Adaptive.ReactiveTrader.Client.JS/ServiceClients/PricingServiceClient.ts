/// <reference path="../typings/rx.js/rx.d.ts"/>
/// <reference path="../Dto/IPriceDto.ts"/>

class PricingServiceClient implements IPricingServiceClient {
    private _connection: IConnection;

    constructor(connection: IConnection) {
        this._connection = connection;
    }

    public getSpotStream(currencyPair: string): Rx.Observable<IPriceDto> {
        return this.getSpotStreamForConnection(currencyPair, this._connection.pricingHubProxy);
    }

    private getSpotStreamForConnection(currencyPair: string, pricingHub: HubProxy) : Rx.Observable<IPriceDto> {
        return Rx.Observable.create<IPriceDto>(observer=> {
            var pricesSubscription = this._connection
                .allPrices
                .subscribe(price=> {
                    if (price.Symbol == currencyPair) {
                        observer.onNext(price);
                    }
                });
            
            console.log("Sending price subscription for currency pair " + currencyPair);

            var subscriptionRequest = new PriceSubscriptionRequestDto();
            subscriptionRequest.CurrencyPair = "EURUSD";

            pricingHub.invoke("SubscribePriceStream", subscriptionRequest)
                .done(_ => console.log("Subscribed to " + currencyPair))
                .fail(ex => observer.onError(ex));

            var unsubsciptionDisposable =  Rx.Disposable.create(()=> {
                pricingHub.invoke("UnsubscribePriceStream", subscriptionRequest)
                    .done(_ => console.log("Unsubscribed from " + currencyPair))
                    .fail(error => console.log("An error occured while sending unsubscription request for " + currencyPair + ":" + error));
            });

            return new Rx.CompositeDisposable([pricesSubscription, unsubsciptionDisposable]);
        });
    }
}
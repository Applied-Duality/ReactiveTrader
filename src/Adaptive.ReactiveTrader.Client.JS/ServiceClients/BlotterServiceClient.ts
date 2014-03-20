 class BlotterServiceClient implements IBlotterServiceClient {
     private _connection: IConnection;
     
     constructor(connection: IConnection) {
         this._connection = connection;
     }

     getTradesStream(): Rx.Observable<TradeDto[]> {
         return this.getTradesStreamFromConnection(this._connection.blotterHubProxy);
     }

     private getTradesStreamFromConnection(blotterHub: HubProxy): Rx.Observable<TradeDto[]> {
         return Rx.Observable.create<TradeDto[]>(observer=> {
             var tradesSubscription = this._connection.allTrades.subscribe(observer);

             console.log("Sending blotter subscription...");
             this._connection.blotterHubProxy.invoke("SubscribeTrades")
                 .done(_=> console.log("Subscribed to blotter"))
                 .fail(ex=> observer.onError(ex));

             var unsubscriptionDisposable = Rx.Disposable.create(()=> {
                 console.log("Sending blotter unsubscription...");

                 this._connection.blotterHubProxy.invoke("UnsubscribeTrades")
                     .done(_=> console.log("Unsubscribed from blotter"))
                     .fail(ex=> console.error("An error occured while unsubscribing from blotter:" + ex));
             });

             return new Rx.CompositeDisposable([tradesSubscription, unsubscriptionDisposable]);
         })
         .publish()
         .refCount();
     }
 }
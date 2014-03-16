/// <reference path="../typings/signalr/signalr.d.ts"/> 
/// <reference path="../typings/rx.js/rx.d.ts"/>

class Connection implements IConnection {
    private _status: Rx.Subject<ConnectionInfo>;
    private _hubConnection: HubConnection;
    private _initialized: boolean;
    private _address: string;
    private _referenceDataHubProxy: HubProxy;
    private _pricingHubProxy: HubProxy;
    private _executionHubProxy: HubProxy;
    private _blotterHubProxy: HubProxy;
    private _allPrices: Rx.Subject<PriceDto>;
    private _currencyPairUpdates: Rx.Subject<CurrencyPairUpdateDto>;

    constructor(address: string, username: string) {
        //var initialConnectionInfo = new ConnectionInfo(ConnectionStatus.Uninitialized, address);
        this._status = new Rx.Subject<ConnectionInfo>();
        this._address = address;
        this._hubConnection = $.hubConnection("http://localhost:8080/signalr");

        // TODO set username header 
        this._hubConnection
            .disconnected(() => this.changeStatus(ConnectionStatus.Closed))
            .connectionSlow(() => this.changeStatus(ConnectionStatus.ConnectionSlow))
            .reconnected(() => this.changeStatus(ConnectionStatus.Reconnected))
            .reconnecting(() => this.changeStatus(ConnectionStatus.Reconnecting))
            .error(error => console.log(error));

        this._referenceDataHubProxy = this._hubConnection.createHubProxy("ReferenceDataHub");
        this._blotterHubProxy = this._hubConnection.createHubProxy("BlotterHub");
        this._executionHubProxy = this._hubConnection.createHubProxy("ExecutionHub");
        this._pricingHubProxy = this._hubConnection.createHubProxy("PricingHub");

        this.installListeners();
    }

    public initialize(): Rx.Observable<{}> {

        return Rx.Observable.create<{}>(observer => {
            this.changeStatus(ConnectionStatus.Connecting);

            console.log("Connecting to " + this._address + "...");
            this._hubConnection.start()
                .done(()=> {
                    this.changeStatus(ConnectionStatus.Connected);
                    observer.onNext(true);
                    console.log("Connected to " + this._address + ".");
                })
                .fail(()=> {
                    this.changeStatus(ConnectionStatus.Closed);
                    var error = "An error occured when starting SignalR connection.";
                    console.log(error);
                    observer.onError(error);
                });
            
            return Rx.Disposable.create(() => {
                console.log("Stoping connection...");
                this._hubConnection.stop();
                console.log("Connection stopped.");
            });
        });
        //.publish()
        //.refCount();
    }

    public changeStatus(newStatus: ConnectionStatus): void {
        this._status.onNext(new ConnectionInfo(newStatus, this.address));
    }

    public get status(): Rx.Observable<ConnectionInfo> {
        return this._status;
    }

    public get address(): string {
        return this._address;
    }

    public get referenceDataHubProxy(): HubProxy {
        return this._referenceDataHubProxy;
    }

    public get pricingHubProxy(): HubProxy {
        return this._pricingHubProxy;
    }

    public get executionHubProxy(): HubProxy {
        return this._executionHubProxy;
    }

    public get blotterHubProxy(): HubProxy {
        return this._blotterHubProxy;
    }

    public get allPrices(): Rx.Observable<PriceDto> {
        return this._allPrices;
    }

    public get currencyPairUpdates(): Rx.Observable<CurrencyPairUpdateDto> {
        return this._currencyPairUpdates;
    }

    private installListeners() {
        this._allPrices = new Rx.Subject<PriceDto>();
        this._currencyPairUpdates = new Rx.Subject<CurrencyPairUpdateDto>();

        this._pricingHubProxy.on("OnNewPrice", price=> this._allPrices.onNext(price));
        this._referenceDataHubProxy.on("OnCurrencyPairUpdate", currencyPairs=> this._currencyPairUpdates.onNext(currencyPairs));
    }
}

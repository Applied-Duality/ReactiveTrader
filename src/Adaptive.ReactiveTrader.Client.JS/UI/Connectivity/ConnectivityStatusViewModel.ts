class ConnectivityStatusViewModel implements IConnectivityStatusViewModel {
    private _priceLatencyRecorder: IPriceLatencyRecorder;

    status: KnockoutObservable<string>;
    uiLatency: KnockoutObservable<number>;
    throughput: KnockoutObservable<number>;
    disconnected: KnockoutObservable<boolean>;

    constructor(reactiveTrader: IReactiveTrader, priceLatencyRecorder: IPriceLatencyRecorder) {
        this._priceLatencyRecorder = priceLatencyRecorder;
        reactiveTrader.connectionStatusStream
            .subscribe(
                status=> this.onStatusChanged(status),
                ex=> console.error("An error occured within the connection status stream", ex));

        Rx.Observable
            .timer(1000, Rx.Scheduler.timeout)
            .repeat()
            .subscribe(_=> this.onTimerTick());

        this.status = ko.observable("Disconnected.");
        this.uiLatency = ko.observable(0);
        this.throughput = ko.observable(0);
        this.disconnected = ko.observable(false);
    }

    private onTimerTick() {
        var current = this._priceLatencyRecorder.getMaxLatencyAndReset();
        if (current == null || current.priceWithMaxLatency == null) return;

        this.uiLatency(current.priceWithMaxLatency.uiProcessingTimeMs);
        this.throughput(current.count);
    }

    private onStatusChanged(connectionInfo: ConnectionInfo) {
        switch(connectionInfo.connectionStatus) {
            case ConnectionStatus.Uninitialized:
            case ConnectionStatus.Connecting:
                this.status("Connecting to " + connectionInfo.server + "...");
                this.disconnected(true);
                break;
            case ConnectionStatus.Reconnected:
            case ConnectionStatus.Connected:
                this.status("Connected to " + connectionInfo.server);
                this.disconnected(false);
                break;
            case ConnectionStatus.ConnectionSlow:
                this.status("Slow connection detected with " + connectionInfo.server);
                this.disconnected(false);
                break;
            case ConnectionStatus.Reconnecting:
                this.status("Reconnecting to " + connectionInfo.server + "...");
                this.disconnected(true);
                break;
            case ConnectionStatus.Closed:
                this.status("Disconnected from " + connectionInfo.server);
                this.disconnected(true);
                break;

        }
    }
} 
class ConnectionProvider implements IConnectionProvider {
    private _username: string;
    private _connectionSequence: Rx.Observable<IConnection>;
    private _servers: string[];
    private _currentIndex: number;

    constructor(username: string, servers: string[]) {
        this._username = username;
        this._servers = servers;

        // TODO shuffle server list
        this._connectionSequence = this.createConnectionSequence();
    }

    public getActiveConnection(): Rx.Observable<IConnection> {
        return this._connectionSequence;
    }

    private createConnectionSequence(): Rx.Observable<IConnection> {
        return Rx.Observable.create<IConnection>(o => {
            console.info("Creating new connection...");

            var connection = this.getNextConnection();

            var statusSubscription = connection.status.subscribe(
                _ => { },
                ex => o.onCompleted(),
                () => {
                    console.info("Status subscription completed");
                    o.onCompleted();
                });

            // TODO if we fail to connect we should not retry straight away to connect to same server, we need some back off
            var connectionSubscription =
                connection.initialize().subscribe(
                    _ => o.onNext(connection),
                    ex => o.onCompleted(),
                    () => o.onCompleted);

                return new Rx.CompositeDisposable(statusSubscription, connectionSubscription);
        });
    }

    private getNextConnection(): IConnection {
        var connection = new Connection(this._servers[this._currentIndex++], this._username);
        if (this._currentIndex == this._servers.length) {
            this._currentIndex = 0;
        }
        return connection;
    }

} 
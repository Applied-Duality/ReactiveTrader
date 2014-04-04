interface IConnectionProvider {
    getActiveConnection(): Rx.Observable<IConnection>;
} 
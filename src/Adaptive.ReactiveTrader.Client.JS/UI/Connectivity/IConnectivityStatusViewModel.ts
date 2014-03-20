interface IConnectivityStatusViewModel {
    status: KnockoutObservable<string>;
    uiLatency: KnockoutObservable<number>;
    throughput: KnockoutObservable<number>;
    disconnected: KnockoutObservable<boolean>;
} 
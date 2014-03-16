interface IConnection {
    status: Rx.Observable<ConnectionInfo>;
    initialize(): Rx.Observable<{}>;
    address: string;
    referenceDataHubProxy: HubProxy;
    pricingHubProxy: HubProxy;
    executionHubProxy: HubProxy;
    blotterHubProxy: HubProxy;
    currencyPairUpdates: Rx.Observable<CurrencyPairUpdateDto>;
    allPrices: Rx.Observable<PriceDto>;
}
interface IPricingServiceClient {
    getSpotStream(currencyPair: string): Rx.Observable<PriceDto>;
} 

/// <reference path="../typings/rx.js/rx.d.ts"/>

interface IPricingServiceClient {
    getSpotStream(currencyPair: string): Rx.Observable<PriceDto>;
} 

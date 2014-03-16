/// <reference path="../typings/rx.js/rx.d.ts"/>
/// <reference path="../Dto/IPriceDto.ts"/>

interface IPricingServiceClient {
    getSpotStream(currencyPair: string): Rx.Observable<IPriceDto>;
} 

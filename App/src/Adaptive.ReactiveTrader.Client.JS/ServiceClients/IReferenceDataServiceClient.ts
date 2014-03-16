/// <reference path="../typings/rx.js/rx.d.ts"/>

interface IReferenceDataServiceClient
{
    getCurrencyPairUpdates() : Rx.Observable<CurrencyPairUpdateDto[]>;
}

/// <reference path="../typings/rx.js/rx.d.ts"/>
/// <reference path="../Dto/ICurrencyPairUpdateDto.ts"/>

interface IReferenceDataServiceClient
{
    getCurrencyPairUpdates() : Rx.Observable<ICurrencyPairUpdateDto>;
}

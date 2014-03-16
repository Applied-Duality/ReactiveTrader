/// <reference path="../typings/rx.js/rx.d.ts"/>
/// <reference path="../Dto/ICurrencyPairUpdateDto.ts"/>
/// <reference path="../Dto/ICurrencyPairDto.ts"/>
/// <reference path="../Dto/UpdateTypeDto.ts"/>

interface IReferenceDataServiceClient
{
    getCurrencyPairUpdates() : Rx.Observable<ICurrencyPairUpdateDto[]>;
}

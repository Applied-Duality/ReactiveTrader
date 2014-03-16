/// <reference path="../typings/rx.js/rx.d.ts"/>
/// <reference path="../typings/signalr/signalr.d.ts"/>
/// <reference path="../Dto/ICurrencyPairUpdateDto.ts"/>
/// <reference path="../Transport/IConnection.ts"/>

class ReferenceDataServiceClient
{
    constructor(connection: IConnection){
        
    }


    getCurrencyPairUpdates() : Rx.Observable<ICurrencyPairUpdateDto> {
        return null;
    }
}

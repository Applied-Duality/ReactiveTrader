/// <reference path="../typings/signalr/signalr.d.ts"/> 
/// <reference path="../typings/rx.js/rx.d.ts"/>

interface IConnection {
    status: Rx.Observable<ConnectionInfo>;
    initialize(): Rx.Observable<{}>;
    address: string;
    referenceDataHubProxy: HubProxy;
    pricingHubProxy: HubProxy;
    executionHubProxy: HubProxy;
    blotterHubProxy: HubProxy;
}
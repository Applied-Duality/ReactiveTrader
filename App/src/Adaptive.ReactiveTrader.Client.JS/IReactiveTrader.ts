 interface IReactiveTrader {
     tradeRepository: ITradeRepository;
     referenceDataRepository: IReferenceDataRepository;

     connectionStatusStream: Rx.Observable<ConnectionInfo>;
     initialize(username: string, server: string): Rx.Observable<{}>;
 } 
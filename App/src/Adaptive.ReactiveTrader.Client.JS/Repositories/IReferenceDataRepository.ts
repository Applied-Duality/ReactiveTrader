interface IReferenceDataRepository {
    getCurrencyPairs(): Rx.Observable<ICurrencyPairUpdate[]>;
} 


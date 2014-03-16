class ReferenceDataRepository implements IReferenceDataRepository {
    private _referenceDataServiceClient: IReferenceDataServiceClient;
    private _currencyPairUpdateFactory: ICurrencyPairUpdateFactory;

    constructor(
        referenceDataServiceClient: IReferenceDataServiceClient,
        currencyPairUpdateFactory: ICurrencyPairUpdateFactory) {

        this._currencyPairUpdateFactory = currencyPairUpdateFactory;
        this._referenceDataServiceClient = referenceDataServiceClient;
    }
    
    getCurrencyPairs(): Rx.Observable<ICurrencyPairUpdate[]> {
        return Rx.Observable.defer(()=> this._referenceDataServiceClient.getCurrencyPairUpdates())
            .where(updates=> updates.length > 0)
            .select(updates=> this.createCurrencyPairUpdates(updates))
            .catch(()=> Rx.Observable.return([]))
            .repeat()
            .publish()
            .refCount();
    }

    private createCurrencyPairUpdates(updates: CurrencyPairUpdateDto[]) : ICurrencyPairUpdate[] {
        var result = [];

        for (var i = 0; i < updates.length; i++) {
            result[i] = this._currencyPairUpdateFactory.create(updates[i]);
        }

        return result;
    }
} 


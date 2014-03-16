interface IReferenceDataServiceClient
{
    getCurrencyPairUpdates() : Rx.Observable<CurrencyPairUpdateDto[]>;
}

class PriceRepository implements IPriceRepository {  
    private _pricingServiceClient: IPricingServiceClient ;
    private _priceFactory: IPriceFactory;
    
    constructor(
        pricingServiceClient: IPricingServiceClient,
        priceFactory: IPriceFactory) {
        this._priceFactory = priceFactory;
        this._pricingServiceClient = pricingServiceClient;
    }
    
    getPrices(currencyPair: ICurrencyPair): Rx.Observable<IPrice> {
        return Rx.Observable.defer(()=> this._pricingServiceClient.getSpotStream(currencyPair.symbol))
            .select(p=> this._priceFactory.create(p, currencyPair))
            .catch(Rx.Observable.return(new StalePrice(currencyPair)))
            .repeat()
            // TODO detect stale 
            .publish()
            .refCount();
    }
} 
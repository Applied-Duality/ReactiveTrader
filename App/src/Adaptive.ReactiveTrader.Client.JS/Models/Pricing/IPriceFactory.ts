interface IPriceFactory {
    create(price: PriceDto, currencyPair: ICurrencyPair): IPrice;
} 
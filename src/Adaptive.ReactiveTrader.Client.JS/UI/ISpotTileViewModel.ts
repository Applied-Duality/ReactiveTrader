interface ISpotTileViewModel {
    symbol: string;
    bid: IOneWayPriceViewModel;
    ask: IOneWayPriceViewModel;
    notional: KnockoutObservable<number>;
    spread: KnockoutObservable<string>;
    dealtCurrency: string;
    movement: KnockoutObservable<PriceMovement>;
    spotDate: KnockoutObservable<string>;
    isSubscribing: KnockoutObservable<boolean>;

    onTrade(trade: ITrade): void;
} 
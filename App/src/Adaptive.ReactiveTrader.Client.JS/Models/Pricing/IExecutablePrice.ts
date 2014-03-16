interface IExecutablePrice
{
    execute(notional: number, dealtCurrency: string): Rx.Observable<ITrade>;
    direction: Direction;
    parent: IPrice;
    rate: number;
} 


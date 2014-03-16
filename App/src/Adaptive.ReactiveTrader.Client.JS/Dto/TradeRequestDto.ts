class TradeRequestDto {
    Symbol: string;
    QuoteId: number;
    SpotRate: number;
    ValueDate: Date;
    Direction: DirectionDto;
    Notional:number;
    DealtCurrency:string;    
}

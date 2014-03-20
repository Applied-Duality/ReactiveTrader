 class TradeViewModel implements ITradeViewModel {
     spotRate: number;
     notional: string;
     direction: string;
     currencyPair: string;
     tradeId: string;
     tradeDate: string;
     tradeStatus: string;
     traderName: string;
     valueDate: string;
     dealtCurrency: string;

     constructor(trade: ITrade) {
         this.spotRate = trade.spotRate;
         this.notional = trade.notional + " " + trade.dealtCurrency;
         this.direction = trade.direction == Direction.Buy ? "Buy" : "Sell";
         this.currencyPair = trade.currencyPair.substring(0, 3) + " / " + trade.currencyPair.substring(3, 6);
         this.tradeId = trade.tradeId.toFixed(0);
         this.tradeDate = trade.tradeDate.toString();
         this.tradeStatus = trade.tradeStatus == TradeStatus.Done ? "Done" : "REJECTED";
         this.traderName = trade.traderName;
         this.valueDate = trade.valueDate.toString();
         this.dealtCurrency = trade.dealtCurrency;
     }
 }
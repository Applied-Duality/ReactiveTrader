using System;

namespace Adaptive.ReactiveTrader.Client.Models
{
    internal class Trade : ITrade
    {
        public string CurrencyPair { get; private set; }
        public Direction Direction { get; private set; }
        public long Notional { get; private set; }
        public decimal SpotRate { get; private set; }
        public TradeStatus TradeStatus { get; private set; }
        public DateTime TradeDate { get; private set; }
        public long TradeId { get; private set; }
        public string TraderName { get; private set; }
        public DateTime ValueDate { get; private set; }

        public Trade(string currencyPair, Direction direction, long notional, decimal spotRate, TradeStatus tradeStatus, DateTime tradeDate, long tradeId, string traderName, DateTime valueDate)
        {
            CurrencyPair = currencyPair;
            Direction = direction;
            Notional = notional;
            SpotRate = spotRate;
            TradeStatus = tradeStatus;
            TradeDate = tradeDate;
            TradeId = tradeId;
            TraderName = traderName;
            ValueDate = valueDate;
        }
    }
}
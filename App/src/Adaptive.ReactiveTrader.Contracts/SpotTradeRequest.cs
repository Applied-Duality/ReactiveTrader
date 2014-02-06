using Adaptive.ReactiveTrader.Contracts.Pricing;

namespace Adaptive.ReactiveTrader.Contracts
{
    public class SpotTradeRequest
    {
        public SpotPrice Price { get; set; }
        public Direction Direction { get; set; }
        public long Notional { get; set; }

        public override string ToString()
        {
            return string.Format("Price: {{{0}}}, Direction: {1}, Notional: {2}", Price, Direction, Notional);
        }
    }
}
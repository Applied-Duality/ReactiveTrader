namespace Adaptive.ReactiveTrader.Shared.Pricing
{
    public class PriceSubscriptionRequestDto
    {
        public string CurrencyPair { get; set; }

        public override string ToString()
        {
            return string.Format("CurrencyPair: {0}", CurrencyPair);
        }
    }
}
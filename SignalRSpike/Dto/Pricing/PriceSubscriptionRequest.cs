namespace Dto.Pricing
{
    public class PriceSubscriptionRequest
    {
        public string CurrencyPair { get; set; }

        public override string ToString()
        {
            return string.Format("CurrencyPair: {0}", CurrencyPair);
        }
    }
}
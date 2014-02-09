namespace Adaptive.ReactiveTrader.Shared.ReferenceData
{
    public class CurrencyPairDto
    {
        public CurrencyPairDto(string symbol, int ratePrecision, int pipsPosition)
        {
            Symbol = symbol;
            RatePrecision = ratePrecision;
            PipsPosition = pipsPosition;
        }

        public string Symbol { get; private set; }
        public int RatePrecision { get; private set; }
        public int PipsPosition { get; private set; }
    }
}
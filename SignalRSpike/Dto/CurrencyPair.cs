namespace Dto
{
    public class CurrencyPair
    {
        public CurrencyPair(string symbol, int ratePrecision, int bigNumberStartIndex)
        {
            Symbol = symbol;
            RatePrecision = ratePrecision;
            BigNumberStartIndex = bigNumberStartIndex;
        }

        public string Symbol { get; private set; }
        public int RatePrecision { get; private set; }
        public int BigNumberStartIndex { get; private set; }
    }
}
namespace Adaptive.ReactiveTrader.Client.UI.SpotTiles
{
    public static class PriceFormatter
    {
        public static FormattedPrice GetFormattedPrice(decimal rate, int precision, int pipsPosition)
        {
            var rateAsString = rate.ToString("0." + new string('0', precision));

            var dotIndex = rateAsString.IndexOf('.');

            var bigFigures = rateAsString.Substring(0, dotIndex + pipsPosition - 1);
            var pips = rateAsString.Substring(dotIndex + pipsPosition - 1, 2);

            var tenthOfPips = "";

            if (precision > pipsPosition)
            {
                tenthOfPips = rateAsString.Substring(dotIndex + pipsPosition + 1, rateAsString.Length - (dotIndex + pipsPosition + 1));
            }

            return new FormattedPrice(bigFigures, pips, tenthOfPips);
        }
    }
}
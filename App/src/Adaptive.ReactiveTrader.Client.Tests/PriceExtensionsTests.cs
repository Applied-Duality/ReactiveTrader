using Adaptive.ReactiveTrader.Client.UI.SpotTiles;
using NUnit.Framework;

namespace Adaptive.ReactiveTrader.Client.Tests
{
    [TestFixture]
    public class PriceExtensionsTests
    {
        [TestCase(1.23456, 5, 4, "1.23", "45", "6")]
        [TestCase(1.2345, 4, 4, "1.23", "45", "")]
        [TestCase(131.015, 3, 2, "131.", "01", "5")]
        public void Test(decimal rate, int precision, int pipsPosition, string bigFigures, string pips, string tenthOfPip)
        {
            var formattedPrice = PriceFormatter.GetFormattedPrice(rate, precision, pipsPosition);

            Assert.AreEqual(bigFigures, formattedPrice.BigFigures);
            Assert.AreEqual(pips, formattedPrice.Pips);
            Assert.AreEqual(tenthOfPip, formattedPrice.TenthOfPip);
        }
    }
}

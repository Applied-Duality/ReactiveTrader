using System;
using System.Diagnostics;
using Adaptive.ReactiveTrader.Shared.DTO.Pricing;
using Adaptive.ReactiveTrader.Shared.DTO.ReferenceData;

namespace Adaptive.ReactiveTrader.Server.ReferenceData
{
    public sealed class RandomWalkCurrencyPairInfo : CurrencyPairInfo
    {
        private static readonly Random _random = new Random();

        public RandomWalkCurrencyPairInfo(CurrencyPairDto currencyPair, decimal sampleRate, bool enabled, string comment) 
            : base(currencyPair, sampleRate, enabled, comment)
        {
        }

        public override PriceDto GenerateNextQuote(PriceDto previousPrice)
        {
            var pow = (decimal)Math.Pow(10, CurrencyPair.RatePrecision);
            var newMid = previousPrice.Mid + _random.Next(-5, 5) / pow;

            return new PriceDto
            {
                Symbol = previousPrice.Symbol,
                QuoteId = previousPrice.QuoteId + 1,
                ValueDate = DateTime.UtcNow.AddDays(2).Date,
                Mid = newMid,
                Ask = newMid + 5 / pow,
                Bid = newMid - 5 / pow,
                CreationTimestamp = Stopwatch.GetTimestamp()
            };
        }
    }
}
using System.Collections.Generic;
using Adaptive.ReactiveTrader.Shared.ReferenceData;

namespace Adaptive.ReactiveTrader.Server.ReferenceData
{
    public class CurrencyPairRepository : ICurrencyPairRepository
    {
        private readonly Dictionary<string, CurrencyPairDto> _currencyPairs = new Dictionary<string, CurrencyPairDto>
        {
            {"EURUSD", new CurrencyPairDto("EURUSD", 5, 4)},
            {"EURGBP", new CurrencyPairDto("EURGBP", 5, 4)},
            {"EURJPY", new CurrencyPairDto("EURJPY", 3, 2)},
        }; 

        public IEnumerable<CurrencyPairDto> GetAllCurrencyPairs()
        {
            return _currencyPairs.Values;
        }

        public CurrencyPairDto GetCurrencyPair(string symbol)
        {
            return _currencyPairs[symbol];
        }

        public bool Exists(string symbol)
        {
            return _currencyPairs.ContainsKey(symbol);
        }
    }
}
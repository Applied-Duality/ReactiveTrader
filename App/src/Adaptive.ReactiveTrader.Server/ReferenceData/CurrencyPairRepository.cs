using System.Collections.Generic;
using Adaptive.ReactiveTrader.Shared.ReferenceData;

namespace Adaptive.ReactiveTrader.Server.ReferenceData
{
    class CurrencyPairRepository : ICurrencyPairRepository
    {
        private readonly Dictionary<string, CurrencyPairDto> _currencyPairs = new Dictionary<string, CurrencyPairDto>
        {
            {"EURUSD", new CurrencyPairDto("EURUSD", 5, 3)},
            {"EURGBP", new CurrencyPairDto("EURGBP", 5, 3)},
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
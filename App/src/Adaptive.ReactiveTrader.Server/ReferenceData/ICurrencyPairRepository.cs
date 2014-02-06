using System.Collections.Generic;
using Adaptive.ReactiveTrader.Contracts;
using Adaptive.ReactiveTrader.Contracts.ReferenceData;

namespace Adaptive.ReactiveTrader.Server.ReferenceData
{
    public interface ICurrencyPairRepository
    {
        IEnumerable<CurrencyPair> GetAllCurrencyPairs();
        CurrencyPair GetCurrencyPair(string symbol);
        bool Exists(string symbol);
    }
}
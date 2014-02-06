using System.Collections.Generic;
using Adaptive.ReactiveTrader.Contracts;

namespace Adaptive.ReactiveTrader.Server
{
    public interface ICurrencyPairRepository
    {
        IEnumerable<CurrencyPair> GetAllCurrencyPairs();
        CurrencyPair GetCurrencyPair(string symbol);
        bool Exists(string symbol);
    }
}
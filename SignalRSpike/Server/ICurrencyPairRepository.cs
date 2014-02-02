using System.Collections.Generic;
using Dto;

namespace Server
{
    public interface ICurrencyPairRepository
    {
        IEnumerable<CurrencyPair> GetAllCurrencyPairs();
        CurrencyPair GetCurrencyPair(string symbol);
        bool Exists(string symbol);
    }
}
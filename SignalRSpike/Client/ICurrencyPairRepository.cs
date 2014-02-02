using System.Collections.Generic;
using System.Threading.Tasks;
using Dto;

namespace Client
{
    interface ICurrencyPairRepository
    {
        Task Initialize();
        IEnumerable<CurrencyPair> GetAllCurrencyPairs();
        CurrencyPair GetCurrencyPair(string symbol);
    }
}

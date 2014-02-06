using System.Collections.Generic;
using System.Threading.Tasks;
using Adaptive.ReactiveTrader.Contracts;

namespace Adaptive.ReactiveTrader.Client
{
    interface ICurrencyPairRepository
    {
        Task Initialize();
        IEnumerable<CurrencyPair> GetAllCurrencyPairs();
        CurrencyPair GetCurrencyPair(string symbol);
    }
}

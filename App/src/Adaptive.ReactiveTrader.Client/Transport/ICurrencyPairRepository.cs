using System.Collections.Generic;
using System.Threading.Tasks;
using Adaptive.ReactiveTrader.Contracts;

namespace Adaptive.ReactiveTrader.Client.Transport
{
    interface ICurrencyPairRepository
    {
        Task Initialize();
        IEnumerable<CurrencyPair> GetAllCurrencyPairs();
        CurrencyPair GetCurrencyPair(string symbol);
    }
}

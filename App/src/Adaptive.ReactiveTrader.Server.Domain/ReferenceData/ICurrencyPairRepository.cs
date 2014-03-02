using System.Collections.Generic;
using Adaptive.ReactiveTrader.Shared.ReferenceData;

namespace Adaptive.ReactiveTrader.Server.ReferenceData
{
    public interface ICurrencyPairRepository
    {
        IEnumerable<CurrencyPairDto> GetAllCurrencyPairs();
        CurrencyPairDto GetCurrencyPair(string symbol);
        bool Exists(string symbol);
        decimal GetSampleRate(string symbol);
        IEnumerable<CurrencyPairInfo> GetAllCurrencyPairInfos();
    }
}
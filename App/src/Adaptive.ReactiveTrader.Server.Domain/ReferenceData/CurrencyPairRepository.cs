using System.Collections.Generic;
using System.Linq;
using Adaptive.ReactiveTrader.Shared.ReferenceData;

namespace Adaptive.ReactiveTrader.Server.ReferenceData
{
    public class CurrencyPairRepository : ICurrencyPairRepository
    {
        private readonly Dictionary<string, CurrencyPairInfo> _currencyPairs = new Dictionary<string, CurrencyPairInfo>
        {
            {"EURUSD", CreateCurrencyPairInfo("EURUSD", 4, 5, 1.3629m)},
            {"USDJPY", CreateCurrencyPairInfo("USDJPY", 2, 3, 102.14m)},
            {"GBPUSD", CreateCurrencyPairInfo("GBPUSD", 4, 5, 1.6395m)},
            {"GBPJPY", CreateCurrencyPairInfo("GBPJPY", 2, 3, 167.67m)},
            {"EURGBP", CreateCurrencyPairInfo("EURGBP", 4, 5, 0.8312m)},
            {"USDCHF", CreateCurrencyPairInfo("USDCHF", 4, 5, 0.897m)},
            {"EURJPY", CreateCurrencyPairInfo("EURJPY", 2, 3, 139.22m)},
            {"EURCHF", CreateCurrencyPairInfo("EURCHF", 4, 5, 1.2224m)},
            {"AUDUSD", CreateCurrencyPairInfo("AUDUSD", 4, 5, 0.8925m)},
            {"NZDUSD", CreateCurrencyPairInfo("NZDUSD", 4, 5, 0.8263m)},
            {"USDCAD", CreateCurrencyPairInfo("USDCAD", 4, 5, 1.1043m)},
            {"EURCAD", CreateCurrencyPairInfo("EURCAD", 4, 5, 1.5062m)},
            {"EURAUD", CreateCurrencyPairInfo("EURAUD", 4, 5, 1.5256m)},
            {"AUDCAD", CreateCurrencyPairInfo("AUDCAD", 4, 5, 0.9873m)},
            {"GBPCHF", CreateCurrencyPairInfo("GBPCHF", 4, 5, 1.4723m)},
            {"CHFJPY", CreateCurrencyPairInfo("CHFJPY", 2, 3, 113.8591m)},
            {"AUDJPY", CreateCurrencyPairInfo("AUDJPY", 2, 3, 91.3133m)},
            {"AUDNZD", CreateCurrencyPairInfo("AUDNZD", 4, 5, 1.0807m)},
            {"CADUSD", CreateCurrencyPairInfo("CADUSD", 4, 5, 0.9054m)},
            {"CADJPY", CreateCurrencyPairInfo("CADJPY", 2, 3, 92.4686m)},
            {"CHFUSD", CreateCurrencyPairInfo("CHFUSD", 4, 5, 1.1148m)},
            {"EURNOK", CreateCurrencyPairInfo("EURNOK", 4, 4, 8.3613m)},
            {"EURSEK", CreateCurrencyPairInfo("EURSEK", 4, 4, 8.8505m)},
        };

        private static CurrencyPairInfo CreateCurrencyPairInfo(string symbol, int pipsPosition, int ratePrecision,
            decimal sampleRate)
        {
            return new CurrencyPairInfo(new CurrencyPairDto(symbol, ratePrecision, pipsPosition), sampleRate);
        }

        public IEnumerable<CurrencyPairDto> GetAllCurrencyPairs()
        {
            return _currencyPairs.Values.Select(cpi => cpi.CurrencyPair);
        }

        public CurrencyPairDto GetCurrencyPair(string symbol)
        {
            return _currencyPairs[symbol].CurrencyPair;
        }

        public decimal GetSampleRate(string symbol)
        {
            return _currencyPairs[symbol].SampleRate;
        }

        public bool Exists(string symbol)
        {
            return _currencyPairs.ContainsKey(symbol);
        }
    }
}
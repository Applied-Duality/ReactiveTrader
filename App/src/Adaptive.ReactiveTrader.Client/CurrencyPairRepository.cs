using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Adaptive.ReactiveTrader.Contracts;
using log4net;

namespace Adaptive.ReactiveTrader.Client
{
    class CurrencyPairRepository : ICurrencyPairRepository
    {
        private readonly ITransport _transport;
        private Dictionary<string, CurrencyPair> _currencyPairs; 
        private static readonly ILog Log = LogManager.GetLogger(typeof(CurrencyPairRepository));

        public CurrencyPairRepository(ITransport transport)
        {
            _transport = transport;
        }

        public async Task Initialize()
        {
            Log.InfoFormat("Loading list of currency pairs...");
            var currencyPairs = await _transport.HubProxy.Invoke<IEnumerable<CurrencyPair>>(ServiceConstants.Server.GetCurrencyPairs);
            _currencyPairs = currencyPairs.ToDictionary(cp => cp.Symbol);
            Log.InfoFormat("Retreived {0} currency pairs.", _currencyPairs.Count);
        }

        public IEnumerable<CurrencyPair> GetAllCurrencyPairs()
        {
            if (_currencyPairs == null) throw new InvalidOperationException("CurrencyPairRepository has not been initialized");

            return _currencyPairs.Values;
        }

        public CurrencyPair GetCurrencyPair(string symbol)
        {
            return _currencyPairs[symbol];
        }
    }
}
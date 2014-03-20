using Adaptive.ReactiveTrader.Server.ReferenceData;
using Adaptive.ReactiveTrader.Shared.UI;

namespace Adaptive.ReactiveTrader.Server
{
    class CurrencyPairViewModel : ViewModelBase, ICurrencyPairViewModel
    {
        private readonly CurrencyPairInfo _currencyPairInfo;
        private readonly ICurrencyPairUpdatePublisher _currencyPairUpdatePublisher;
        public string Comment { get; private set; }
        public string Symbol { get; private set; }

        public CurrencyPairViewModel(CurrencyPairInfo currencyPairInfo, ICurrencyPairUpdatePublisher currencyPairUpdatePublisher)
        {
            _currencyPairInfo = currencyPairInfo;
            _currencyPairUpdatePublisher = currencyPairUpdatePublisher;
            Symbol = currencyPairInfo.CurrencyPair.Symbol;
            Available = currencyPairInfo.Enabled;
            Stale = currencyPairInfo.Stale;
            Comment = currencyPairInfo.Comment;
        }

        public bool Available
        {
            get { return _currencyPairInfo.Enabled; }
            set
            {
                if (value)
                {
                    _currencyPairUpdatePublisher.AddCurrencyPair(_currencyPairInfo.CurrencyPair);
                }
                else
                {
                    _currencyPairUpdatePublisher.RemoveCurrencyPair(_currencyPairInfo.CurrencyPair);   
                }
                
                _currencyPairInfo.Enabled = value;
            }
        }

        public bool Stale
        {
            get { return _currencyPairInfo.Stale; }
            set
            {
                _currencyPairInfo.Stale = value;
            }
        }
    }
}
using Adaptive.ReactiveTrader.Server.ReferenceData;
using Adaptive.ReactiveTrader.Shared;
using Adaptive.ReactiveTrader.Shared.ReferenceData;
using Adaptive.ReactiveTrader.Shared.UI;

namespace Adaptive.ReactiveTrader.Server
{
    class CurrencyPairViewModel : ViewModelBase, ICurrencyPairViewModel
    {
        private readonly CurrencyPairInfo _currencyPairInfo;
        private readonly ICurrencyPairUpdatePublisher _currencyPairUpdatePublisher;
        private bool _available;

        public CurrencyPairViewModel(CurrencyPairInfo currencyPairInfo, ICurrencyPairUpdatePublisher currencyPairUpdatePublisher)
        {
            _currencyPairInfo = currencyPairInfo;
            _currencyPairUpdatePublisher = currencyPairUpdatePublisher;
            Symbol = currencyPairInfo.CurrencyPair.Symbol;
            Available = currencyPairInfo.Enabled;
            Stale = currencyPairInfo.Stale;
        }

        public string Symbol { get; private set; }

        public bool Available
        {
            get { return _available; }
            set
            {
                // TODO refactor this code (this logic should be in the server)
                var update = new CurrencyPairUpdateDto
                {
                    CurrencyPair = _currencyPairInfo.CurrencyPair,
                    UpdateType = value ? UpdateTypeDto.Added : UpdateTypeDto.Removed
                };

                _currencyPairUpdatePublisher.Publish(update);

                _available = value;
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
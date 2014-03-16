using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using Adaptive.ReactiveTrader.Client.Concurrency;
using Adaptive.ReactiveTrader.Client.Domain;
using Adaptive.ReactiveTrader.Client.Domain.Models.Execution;
using Adaptive.ReactiveTrader.Client.Domain.Repositories;
using Adaptive.ReactiveTrader.Shared.UI;
using log4net;
using PropertyChanged;

namespace Adaptive.ReactiveTrader.Client.UI.Blotter
{
    [ImplementPropertyChanged]
    public class BlotterViewModel : ViewModelBase, IBlotterViewModel
    {
        private readonly ITradeRepository _tradeRepository;
        private readonly Func<ITrade, ITradeViewModel> _tradeViewModelFactory;
        private readonly IConcurrencyService _concurrencyService;
        public ObservableCollection<ITradeViewModel> Trades { get; private set; }

        private static readonly ILog Log = LogManager.GetLogger(typeof(BlotterViewModel));
        private bool _stale;

        public BlotterViewModel(IReactiveTrader reactiveTrader,
                                Func<ITrade, ITradeViewModel> tradeViewModelFactory,
                                IConcurrencyService concurrencyService)
        {
            _tradeRepository = reactiveTrader.TradeRepository;
            _tradeViewModelFactory = tradeViewModelFactory;
            _concurrencyService = concurrencyService;
            Trades = new ObservableCollection<ITradeViewModel>();

            LoadTrades();
        }

        private void LoadTrades()
        {
            _tradeRepository.GetTrades()
                            .ObserveOn(_concurrencyService.Dispatcher)
                            .SubscribeOn(_concurrencyService.ThreadPool)
                            .Subscribe(
                                AddTrades,
                                ex => Log.Error("An error occured within the trade stream", ex));
        }

        private void AddTrades(IEnumerable<ITrade> trades)
        {
            var allTrades = trades as IList<ITrade> ?? trades.ToList();
            if (!allTrades.Any())
            {
                // empty list of trades means we are disconnected
                _stale = true;
            }
            else
            {
                if (_stale)
                {
                    Trades.Clear();
                    _stale = false;
                }
            }

            allTrades.ForEach(trade =>
                {
                    var tradeViewMode = _tradeViewModelFactory(trade);
                    Trades.Add(tradeViewMode);
                });
        }
    }
}

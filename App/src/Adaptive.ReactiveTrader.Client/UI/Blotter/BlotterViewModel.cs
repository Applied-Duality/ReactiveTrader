using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using Adaptive.ReactiveTrader.Client.Models;
using Adaptive.ReactiveTrader.Client.Repositories;
using log4net;

namespace Adaptive.ReactiveTrader.Client.UI.Blotter
{
    public class BlotterViewModel : ViewModelBase, IBlotterViewModel
    {
        private readonly ITradeRepository _tradeRepository;
        private readonly Func<ITrade, ITradeViewModel> _tradeViewModelFactory;
        public ObservableCollection<ITradeViewModel> Trades { get; private set; }

        private static readonly ILog Log = LogManager.GetLogger(typeof(BlotterViewModel));
        private IDisposable _tradesSubscription;

        public BlotterViewModel(ITradeRepository tradeRepository, Func<ITrade, ITradeViewModel> tradeViewModelFactory)
        {
            _tradeRepository = tradeRepository;
            _tradeViewModelFactory = tradeViewModelFactory;
            Trades = new ObservableCollection<ITradeViewModel>();

            LoadTrades();
        }

        private void LoadTrades()
        {
            _tradesSubscription = _tradeRepository.GetTrades()
                .ObserveOnDispatcher()
                .Subscribe(
                AddTrades,
                ex => Log.Error("An error occured within the trade stream", ex));
        }

        private void AddTrades(IEnumerable<ITrade> trades)
        {
            foreach (var trade in trades)
            {
                var tradeViewMode = _tradeViewModelFactory(trade);
                Trades.Add(tradeViewMode);
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using Adaptive.ReactiveTrader.Client.Domain;
using Adaptive.ReactiveTrader.Client.Domain.Models;
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
        public ObservableCollection<ITradeViewModel> Trades { get; private set; }

        private static readonly ILog Log = LogManager.GetLogger(typeof(BlotterViewModel));
        private IDisposable _tradesSubscription;

        public BlotterViewModel(IReactiveTrader reactiveTrader, Func<ITrade, ITradeViewModel> tradeViewModelFactory)
        {
            _tradeRepository = reactiveTrader.TradeRepository;
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
            var allTrades = trades as IList<ITrade> ?? trades.ToList();
            if (!allTrades.Any())
            {
                // TODO we currently use an empty enumerable to represent blotter disconnected, we need a proper representation for that 
                Trades.Clear();
            }

            foreach (var trade in allTrades)
            {
                var tradeViewMode = _tradeViewModelFactory(trade);
                Trades.Add(tradeViewMode);
            }
        }
    }
}
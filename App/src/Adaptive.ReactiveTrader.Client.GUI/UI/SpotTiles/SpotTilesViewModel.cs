using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using Adaptive.ReactiveTrader.Client.Concurrency;
using Adaptive.ReactiveTrader.Client.Domain;
using Adaptive.ReactiveTrader.Client.Domain.Models;
using Adaptive.ReactiveTrader.Client.Domain.Models.ReferenceData;
using Adaptive.ReactiveTrader.Shared.UI;
using Adaptive.ReactiveTrader.Client.Domain.Repositories;
using log4net;
using PropertyChanged;

namespace Adaptive.ReactiveTrader.Client.UI.SpotTiles
{
    [ImplementPropertyChanged]
    public class SpotTilesViewModel : ViewModelBase, ISpotTilesViewModel
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof (SpotTilesViewModel));

        public ObservableCollection<ISpotTileViewModel> SpotTiles { get; private set; }
        private readonly IReferenceDataRepository _referenceDataRepository;
        private readonly Func<ICurrencyPair, ISpotTileViewModel> _spotTileFactory;
        private readonly ISchedulerProvider _schedulerProvider;
        private readonly ISpotTileViewModel _config;

        public SpotTilesViewModel(IReactiveTrader reactiveTrader,
            Func<ICurrencyPair, ISpotTileViewModel> spotTileFactory,
            ISchedulerProvider schedulerProvider)
        {
            _referenceDataRepository = reactiveTrader.ReferenceData;
            _spotTileFactory = spotTileFactory;
            _schedulerProvider = schedulerProvider;

            SpotTiles = new ObservableCollection<ISpotTileViewModel>();

            _config = spotTileFactory(null);
            _config.ToConfig();

            SpotTiles.Add(_config);
            _config.Config.PropertyChanged += (_, e) => { if (e.PropertyName == "SubscriptionMode")
            {
                SpotTiles.Where(vm => vm.Pricing != null).ForEach(vm => vm.Pricing.SubscriptionMode = _config.Config.SubscriptionMode);
            }};

            LoadSpotTiles();
        }

        private void LoadSpotTiles()
        {
            _referenceDataRepository.GetCurrencyPairs()
                .ObserveOn(_schedulerProvider.Dispatcher)
                .SubscribeOn(_schedulerProvider.ThreadPool)
                .Subscribe(
                    currencyPairs => currencyPairs.ForEach(HandleCurrencyPairUpdate),
                    error => Log.Error("Failed to get currencies", error));
        }

        private void HandleCurrencyPairUpdate(ICurrencyPairUpdate update)
        {
            var spotTileViewModel = SpotTiles.FirstOrDefault(stvm => stvm.CurrencyPair == update.CurrencyPair.Symbol);
            if (update.UpdateType == UpdateType.Add)
            {
                if (spotTileViewModel != null)
                {
                    // we already have a tile for this currency pair
                    return;
                }

                var spotTile = _spotTileFactory(update.CurrencyPair);
                SpotTiles.Add(spotTile);
            }
            else
            {
                if (spotTileViewModel != null)
                {
                    SpotTiles.Remove(spotTileViewModel);
                    spotTileViewModel.Dispose();
                }
            }
        }
    }
}

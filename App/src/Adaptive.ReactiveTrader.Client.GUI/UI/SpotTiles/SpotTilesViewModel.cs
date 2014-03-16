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
        private readonly Func<ICurrencyPair, SpotTileSubscriptionMode, ISpotTileViewModel> _spotTileFactory;
        private readonly IConcurrencyService _concurrencyService;
        private readonly ISpotTileViewModel _config;

        public SpotTilesViewModel(IReactiveTrader reactiveTrader,
            Func<ICurrencyPair, SpotTileSubscriptionMode, ISpotTileViewModel> spotTileFactory,
            IConcurrencyService concurrencyService)
        {
            _referenceDataRepository = reactiveTrader.ReferenceData;
            _spotTileFactory = spotTileFactory;
            _concurrencyService = concurrencyService;

            SpotTiles = new ObservableCollection<ISpotTileViewModel>();

            _config = spotTileFactory(null, SpotTileSubscriptionMode.Conflate);
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
            _referenceDataRepository.GetCurrencyPairsStream()
                .ObserveOn(_concurrencyService.Dispatcher)
                .SubscribeOn(_concurrencyService.ThreadPool)
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

                var spotTile = _spotTileFactory(update.CurrencyPair, _config.Config.SubscriptionMode);
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

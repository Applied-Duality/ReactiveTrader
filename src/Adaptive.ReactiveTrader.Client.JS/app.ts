window.onload = () => {
    var reactiveTrader: IReactiveTrader = new ReactiveTrader();

    reactiveTrader
        .initialize("olivier", "http://localhost:800")
        .subscribe(_=> {
            reactiveTrader.connectionStatusStream.subscribe(s=> console.log("Connection status: " + s));
            
            var priceLatencyRecorder = new PriceLatencyRecorder();
            var spotTileViewModelFactory = new SpotTileViewModelFactory(priceLatencyRecorder);
            var spotTilesViewModel = new SpotTilesViewModel(reactiveTrader.referenceDataRepository, spotTileViewModelFactory);
            var blotterViewModel = new BlotterViewModel(reactiveTrader.tradeRepository);
            var connectivityStatusViewModel = new ConnectivityStatusViewModel(reactiveTrader, priceLatencyRecorder);
            var shellViewModel = new ShellViewModel(spotTilesViewModel, blotterViewModel, connectivityStatusViewModel);

            ko.applyBindings(shellViewModel);
        },
            console.error);
};
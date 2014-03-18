window.onload = () => {
    var reactiveTrader: IReactiveTrader = new ReactiveTrader();

    reactiveTrader
        .initialize("olivier", "http://localhost:800")
        .subscribe(_=> {
            reactiveTrader.connectionStatusStream.subscribe(s=> console.log("Connection status: " + s));

            var appViewModel = new AppViewModel(reactiveTrader);

            ko.applyBindings(appViewModel);
        },
            ex=> console.error(ex));
};
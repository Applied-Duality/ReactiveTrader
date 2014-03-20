class ShellViewModel {
    spotTiles: ISpotTilesViewModel;
    blotter: IBlotterViewModel;
    connectivityStatus: IConnectivityStatusViewModel;

    constructor(
        spotTiles: ISpotTilesViewModel,
        blotter: IBlotterViewModel,
        connectivityStatus: IConnectivityStatusViewModel) {
        this.spotTiles = spotTiles;
        this.blotter = blotter;
        this.connectivityStatus = connectivityStatus;
    }
} 
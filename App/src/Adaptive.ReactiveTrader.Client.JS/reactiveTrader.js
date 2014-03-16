/// <reference path="../typings/signalr/signalr.d.ts"/>
/// <reference path="../typings/rx.js/rx.d.ts"/>
var Connection = (function () {
    function Connection(address, username) {
        var _this = this;
        //var initialConnectionInfo = new ConnectionInfo(ConnectionStatus.Uninitialized, address);
        this._status = new Rx.Subject();
        this._address = address;
        this._hubConnection = $.hubConnection("http://localhost:8080/signalr");

        // TODO set username header
        this._hubConnection.disconnected(function () {
            return _this.changeStatus(5 /* Closed */);
        }).connectionSlow(function () {
            return _this.changeStatus(2 /* ConnectionSlow */);
        }).reconnected(function () {
            return _this.changeStatus(4 /* Reconnected */);
        }).reconnecting(function () {
            return _this.changeStatus(3 /* Reconnecting */);
        }).error(function (error) {
            return console.log(error);
        });

        this._referenceDataHubProxy = this._hubConnection.createHubProxy("ReferenceDataHub");
        this._blotterHubProxy = this._hubConnection.createHubProxy("BlotterHub");
        this._executionHubProxy = this._hubConnection.createHubProxy("ExecutionHub");
        this._pricingHubProxy = this._hubConnection.createHubProxy("PricingHub");
    }
    Connection.prototype.initialize = function () {
        var _this = this;
        return Rx.Observable.create(function (observer) {
            _this.changeStatus(0 /* Connecting */);

            console.log("Connecting to " + _this._address + "...");
            _this._hubConnection.start().done(function () {
                _this.changeStatus(1 /* Connected */);
                observer.onNext(true);
                console.log("Connected to " + _this._address + ".");
            }).fail(function () {
                _this.changeStatus(5 /* Closed */);
                var error = "An error occured when starting SignalR connection.";
                console.log(error);
                observer.onError(error);
            });

            return Rx.Disposable.create(function () {
                console.log("Stoping connection...");
                _this._hubConnection.stop();
                console.log("Connection stopped.");
            });
        });
        //.publish()
        //.refCount();
    };

    Connection.prototype.changeStatus = function (newStatus) {
        this._status.onNext(new ConnectionInfo(newStatus, this.address));
    };

    Object.defineProperty(Connection.prototype, "status", {
        get: function () {
            return this._status;
        },
        enumerable: true,
        configurable: true
    });

    Object.defineProperty(Connection.prototype, "address", {
        get: function () {
            return this._address;
        },
        enumerable: true,
        configurable: true
    });

    Object.defineProperty(Connection.prototype, "referenceDataHubProxy", {
        get: function () {
            return this._referenceDataHubProxy;
        },
        enumerable: true,
        configurable: true
    });

    Object.defineProperty(Connection.prototype, "pricingHubProxy", {
        get: function () {
            return this._pricingHubProxy;
        },
        enumerable: true,
        configurable: true
    });

    Object.defineProperty(Connection.prototype, "executionHubProxy", {
        get: function () {
            return this._executionHubProxy;
        },
        enumerable: true,
        configurable: true
    });

    Object.defineProperty(Connection.prototype, "blotterHubProxy", {
        get: function () {
            return this._blotterHubProxy;
        },
        enumerable: true,
        configurable: true
    });
    return Connection;
})();
/// <reference path="typings/signalr/signalr.d.ts" />
/// <reference path="typings/rx.js/rx.d.ts"/>
/// <reference path="Transport/Connection.ts" />
window.onload = function () {
    var el = document.getElementById('content');

    var connection = new Connection("http://localhost:8080", "Olivier");

    connection.status.subscribe(function (status) {
        return console.log("Connection status changed to " + status);
    }, function (ex) {
        return console.log(ex);
    });

    connection.initialize().subscribe(function (_) {
        return console.log("Connected");
    }, function (ex) {
        return console.log(ex);
    });
};
var UpdateTypeDto;
(function (UpdateTypeDto) {
    UpdateTypeDto[UpdateTypeDto["Added"] = 0] = "Added";
    UpdateTypeDto[UpdateTypeDto["Removed"] = 1] = "Removed";
})(UpdateTypeDto || (UpdateTypeDto = {}));
/// <reference path="../typings/signalr/signalr.d.ts"/>
/// <reference path="../typings/rx.js/rx.d.ts"/>
/// <reference path="../typings/rx.js/rx.d.ts"/>
/// <reference path="../typings/signalr/signalr.d.ts"/>
/// <reference path="../Dto/ICurrencyPairUpdateDto.ts"/>
/// <reference path="../Transport/IConnection.ts"/>
var ReferenceDataServiceClient = (function () {
    function ReferenceDataServiceClient(connection) {
    }
    ReferenceDataServiceClient.prototype.getCurrencyPairUpdates = function () {
        return null;
    };
    return ReferenceDataServiceClient;
})();
/// <reference path="../typings/rx.js/rx.d.ts"/>
/// <reference path="../Dto/ICurrencyPairUpdateDto.ts"/>
var ConnectionStatus;
(function (ConnectionStatus) {
    ConnectionStatus[ConnectionStatus["Connecting"] = 0] = "Connecting";
    ConnectionStatus[ConnectionStatus["Connected"] = 1] = "Connected";
    ConnectionStatus[ConnectionStatus["ConnectionSlow"] = 2] = "ConnectionSlow";
    ConnectionStatus[ConnectionStatus["Reconnecting"] = 3] = "Reconnecting";
    ConnectionStatus[ConnectionStatus["Reconnected"] = 4] = "Reconnected";
    ConnectionStatus[ConnectionStatus["Closed"] = 5] = "Closed";
    ConnectionStatus[ConnectionStatus["Uninitialized"] = 6] = "Uninitialized";
})(ConnectionStatus || (ConnectionStatus = {}));
/// <reference path="../typings/signalr/signalr.d.ts"/>
/// <reference path="../typings/rx.js/rx.d.ts"/>
var ConnectionInfo = (function () {
    function ConnectionInfo(connectionStatus, server) {
        this.connectionStatus = connectionStatus;
        this.server = server;
    }
    ConnectionInfo.prototype.toString = function () {
        return "ConnectionStatus: " + this.connectionStatus + ", Server: " + this.server;
    };
    return ConnectionInfo;
})();
//# sourceMappingURL=reactiveTrader.js.map

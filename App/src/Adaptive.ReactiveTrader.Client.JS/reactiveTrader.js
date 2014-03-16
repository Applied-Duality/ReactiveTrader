var Connection = (function () {
    function Connection(address, username) {
        var _this = this;
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

        this.installListeners();
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

    Object.defineProperty(Connection.prototype, "allPrices", {
        get: function () {
            return this._allPrices;
        },
        enumerable: true,
        configurable: true
    });

    Object.defineProperty(Connection.prototype, "currencyPairUpdates", {
        get: function () {
            return this._currencyPairUpdates;
        },
        enumerable: true,
        configurable: true
    });

    Connection.prototype.installListeners = function () {
        var _this = this;
        this._allPrices = new Rx.Subject();
        this._currencyPairUpdates = new Rx.Subject();

        this._pricingHubProxy.on("OnNewPrice", function (price) {
            return _this._allPrices.onNext(price);
        });
        this._referenceDataHubProxy.on("OnCurrencyPairUpdate", function (currencyPairs) {
            return _this._currencyPairUpdates.onNext(currencyPairs);
        });
    };
    return Connection;
})();
var ReferenceDataServiceClient = (function () {
    function ReferenceDataServiceClient(connection) {
        this._connection = connection;
    }
    ReferenceDataServiceClient.prototype.getCurrencyPairUpdates = function () {
        return this.getTradesForConnection(this._connection.referenceDataHubProxy);
    };

    ReferenceDataServiceClient.prototype.getTradesForConnection = function (referenceDataHubProxy) {
        var _this = this;
        return Rx.Observable.create(function (observer) {
            var currencyPairUpdateSubscription = _this._connection.currencyPairUpdates.subscribe(function (currencyPairUpdate) {
                return observer.onNext([currencyPairUpdate]);
            });

            console.log("Sending currency pair subscription...");

            referenceDataHubProxy.invoke("GetCurrencyPairs").done(function (currencyPairs) {
                observer.onNext(currencyPairs);
                console.log("Subscribed to currency pairs and received " + currencyPairs.length + " currency pairs.");
            }).fail(function (ex) {
                return observer.onError(ex);
            });

            return currencyPairUpdateSubscription;
        });
        //.publish()
        //.refCount();
    };
    return ReferenceDataServiceClient;
})();
/// <reference path="typings/signalr/signalr.d.ts" />
/// <reference path="typings/rx.js/rx.d.ts"/>
/// <reference path="Transport/Connection.ts" />
/// <reference path="ServiceClients/ReferenceDataServiceClient.ts" />
window.onload = function () {
    var el = document.getElementById('content');

    var connection = new Connection("http://localhost:8080", "Olivier");

    connection.status.subscribe(function (status) {
        return console.log("Connection status changed to " + status);
    }, function (ex) {
        return console.log(ex);
    });

    connection.initialize().subscribe(function (_) {
        console.log("Connected");
        var refData = new ReferenceDataServiceClient(connection);
        refData.getCurrencyPairUpdates().subscribe(function (currencyPairs) {
            return console.log(currencyPairs);
        }, function (ex) {
            return console.error(ex);
        });

        var pricing = new PricingServiceClient(connection);
        pricing.getSpotStream("EURUSD").subscribe(function (price) {
            return console.log(price.Bid + "/" + price.Ask);
        }, function (ex) {
            return console.error(ex);
        });
    }, function (ex) {
        return console.log(ex);
    });
};
var TradeStatus;
(function (TradeStatus) {
    TradeStatus[TradeStatus["Done"] = 0] = "Done";
    TradeStatus[TradeStatus["Rejected"] = 1] = "Rejected";
})(TradeStatus || (TradeStatus = {}));
var TradeDto = (function () {
    function TradeDto() {
    }
    return TradeDto;
})();
var DirectionDto;
(function (DirectionDto) {
    DirectionDto[DirectionDto["Buy"] = 0] = "Buy";
    DirectionDto[DirectionDto["Sell"] = 1] = "Sell";
})(DirectionDto || (DirectionDto = {}));
var TradeRequestDto = (function () {
    function TradeRequestDto() {
    }
    return TradeRequestDto;
})();
var PriceSubscriptionRequestDto = (function () {
    function PriceSubscriptionRequestDto() {
    }
    return PriceSubscriptionRequestDto;
})();
var PriceDto = (function () {
    function PriceDto() {
    }
    return PriceDto;
})();
var UpdateTypeDto;
(function (UpdateTypeDto) {
    UpdateTypeDto[UpdateTypeDto["Added"] = 0] = "Added";
    UpdateTypeDto[UpdateTypeDto["Removed"] = 1] = "Removed";
})(UpdateTypeDto || (UpdateTypeDto = {}));
var CurrencyPairDto = (function () {
    function CurrencyPairDto() {
    }
    return CurrencyPairDto;
})();
var CurrencyPairUpdateDto = (function () {
    function CurrencyPairUpdateDto() {
    }
    return CurrencyPairUpdateDto;
})();
var ExecutionServiceClient = (function () {
    function ExecutionServiceClient(connection) {
        this._connection = connection;
    }
    ExecutionServiceClient.prototype.execute = function (tradeRequest) {
        return this.executeForConnection(tradeRequest, this._connection.executionHubProxy);
    };

    ExecutionServiceClient.prototype.executeForConnection = function (tradeRequest, executionHub) {
        return Rx.Observable.create(function (observer) {
            executionHub.invoke("Execute", tradeRequest).done(function (trade) {
                return observer.onNext(trade);
            }).fail(function (error) {
                return observer.onError(error);
            });

            return Rx.Disposable.empty;
        });
    };
    return ExecutionServiceClient;
})();
var PricingServiceClient = (function () {
    function PricingServiceClient(connection) {
        this._connection = connection;
    }
    PricingServiceClient.prototype.getSpotStream = function (currencyPair) {
        return this.getSpotStreamForConnection(currencyPair, this._connection.pricingHubProxy);
    };

    PricingServiceClient.prototype.getSpotStreamForConnection = function (currencyPair, pricingHub) {
        var _this = this;
        return Rx.Observable.create(function (observer) {
            var pricesSubscription = _this._connection.allPrices.subscribe(function (price) {
                if (price.Symbol == currencyPair) {
                    observer.onNext(price);
                }
            });

            console.log("Sending price subscription for currency pair " + currencyPair);

            var subscriptionRequest = new PriceSubscriptionRequestDto();
            subscriptionRequest.CurrencyPair = "EURUSD";

            pricingHub.invoke("SubscribePriceStream", subscriptionRequest).done(function (_) {
                return console.log("Subscribed to " + currencyPair);
            }).fail(function (ex) {
                return observer.onError(ex);
            });

            var unsubsciptionDisposable = Rx.Disposable.create(function () {
                pricingHub.invoke("UnsubscribePriceStream", subscriptionRequest).done(function (_) {
                    return console.log("Unsubscribed from " + currencyPair);
                }).fail(function (error) {
                    return console.log("An error occured while sending unsubscription request for " + currencyPair + ":" + error);
                });
            });

            return new Rx.CompositeDisposable([pricesSubscription, unsubsciptionDisposable]);
        });
    };
    return PricingServiceClient;
})();
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

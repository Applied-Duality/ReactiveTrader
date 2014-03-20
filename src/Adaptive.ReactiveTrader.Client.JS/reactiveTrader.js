window.onload = function () {
    var reactiveTrader = new ReactiveTrader();

    reactiveTrader.initialize("olivier", "http://localhost:800").subscribe(function (_) {
        reactiveTrader.connectionStatusStream.subscribe(function (s) {
            return console.log("Connection status: " + s);
        });

        var priceLatencyRecorder = new PriceLatencyRecorder();
        var spotTileViewModelFactory = new SpotTileViewModelFactory(priceLatencyRecorder);
        var spotTilesViewModel = new SpotTilesViewModel(reactiveTrader.referenceDataRepository, spotTileViewModelFactory);
        var blotterViewModel = new BlotterViewModel(reactiveTrader.tradeRepository);
        var connectivityStatusViewModel = new ConnectivityStatusViewModel(reactiveTrader, priceLatencyRecorder);
        var shellViewModel = new ShellViewModel(spotTilesViewModel, blotterViewModel, connectivityStatusViewModel);

        ko.applyBindings(shellViewModel);
    }, console.error);
};
var MaxLatency = (function () {
    function MaxLatency(count, priceWithMaxLatency) {
        this.count = count;
        this.priceWithMaxLatency = priceWithMaxLatency;
    }
    return MaxLatency;
})();
var PriceLatencyRecorder = (function () {
    function PriceLatencyRecorder() {
    }
    PriceLatencyRecorder.prototype.record = function (price) {
        var priceLatency = price;
        if (priceLatency != null) {
            priceLatency.displayedOnUi();

            this._count++;
            if (this._maxLatency == null || priceLatency.uiProcessingTimeMs > this._maxLatency.uiProcessingTimeMs) {
                this._maxLatency = priceLatency;
            }
        }
    };

    PriceLatencyRecorder.prototype.getMaxLatencyAndReset = function () {
        var result = new MaxLatency(this._count, this._maxLatency);
        this._count = 0;
        this._maxLatency = null;
        return result;
    };
    return PriceLatencyRecorder;
})();
var BlotterViewModel = (function () {
    function BlotterViewModel(tradeRepository) {
        this._tradeRepository = tradeRepository;
        this.trades = ko.observableArray([]);

        this.loadTrades();
    }
    BlotterViewModel.prototype.loadTrades = function () {
        var _this = this;
        this._tradeRepository.getTrades().subscribe(function (trades) {
            return _this.addTrades(trades);
        }, function (ex) {
            return console.error("an error occured within the trade stream", ex);
        });
    };

    BlotterViewModel.prototype.addTrades = function (trades) {
        var _this = this;
        if (trades.length == 0) {
            // empty list of trades means we are disconnected
            this._stale = true;
        } else {
            if (this._stale) {
                this.trades.removeAll();
                this._stale = false;
            }
        }

        trades.forEach(function (t) {
            var tradeViewModel = new TradeViewModel(t);
            _this.trades.push(tradeViewModel);
        });
    };
    return BlotterViewModel;
})();
var TradeViewModel = (function () {
    function TradeViewModel(trade) {
        this.spotRate = trade.spotRate;
        this.notional = trade.notional + " " + trade.dealtCurrency;
        this.direction = trade.direction == 0 /* Buy */ ? "Buy" : "Sell";
        this.currencyPair = trade.currencyPair.substring(0, 3) + " / " + trade.currencyPair.substring(3, 6);
        this.tradeId = trade.tradeId.toFixed(0);
        this.tradeDate = trade.tradeDate.toString();
        this.tradeStatus = trade.tradeStatus == 0 /* Done */ ? "Done" : "REJECTED";
        this.traderName = trade.traderName;
        this.valueDate = trade.valueDate.toString();
        this.dealtCurrency = trade.dealtCurrency;
    }
    return TradeViewModel;
})();
var ConnectivityStatusViewModel = (function () {
    function ConnectivityStatusViewModel(reactiveTrader, priceLatencyRecorder) {
        var _this = this;
        this._priceLatencyRecorder = priceLatencyRecorder;
        reactiveTrader.connectionStatusStream.subscribe(function (status) {
            return _this.onStatusChanged(status);
        }, function (ex) {
            return console.error("An error occured within the connection status stream", ex);
        });

        Rx.Observable.timer(1000, Rx.Scheduler.timeout).repeat().subscribe(function (_) {
            return _this.onTimerTick();
        });

        this.status = ko.observable("Disconnected.");
        this.uiLatency = ko.observable(0);
        this.throughput = ko.observable(0);
        this.disconnected = ko.observable(false);
    }
    ConnectivityStatusViewModel.prototype.onTimerTick = function () {
        var current = this._priceLatencyRecorder.getMaxLatencyAndReset();
        if (current == null || current.priceWithMaxLatency == null)
            return;

        this.uiLatency(current.priceWithMaxLatency.uiProcessingTimeMs);
        this.throughput(current.count);
    };

    ConnectivityStatusViewModel.prototype.onStatusChanged = function (connectionInfo) {
        switch (connectionInfo.connectionStatus) {
            case 6 /* Uninitialized */:
            case 0 /* Connecting */:
                this.status("Connecting to " + connectionInfo.server + "...");
                this.disconnected(true);
                break;
            case 4 /* Reconnected */:
            case 1 /* Connected */:
                this.status("Connected to " + connectionInfo.server);
                this.disconnected(false);
                break;
            case 2 /* ConnectionSlow */:
                this.status("Slow connection detected with " + connectionInfo.server);
                this.disconnected(false);
                break;
            case 3 /* Reconnecting */:
                this.status("Reconnecting to " + connectionInfo.server + "...");
                this.disconnected(true);
                break;
            case 5 /* Closed */:
                this.status("Disconnected from " + connectionInfo.server);
                this.disconnected(true);
                break;
        }
    };
    return ConnectivityStatusViewModel;
})();
var ShellViewModel = (function () {
    function ShellViewModel(spotTiles, blotter, connectivityStatus) {
        this.spotTiles = spotTiles;
        this.blotter = blotter;
        this.connectivityStatus = connectivityStatus;
    }
    return ShellViewModel;
})();
var OneWayPriceViewModel = (function () {
    function OneWayPriceViewModel(parent, direction) {
        this._parent = parent;
        this.direction = direction == 0 /* Buy */ ? "BUY" : "SELL";

        this.bigFigures = ko.observable("");
        this.pips = ko.observable("");
        this.tenthOfPips = ko.observable("");
        this.isExecuting = ko.observable(false);
        this.isStale = ko.observable(true);
    }
    OneWayPriceViewModel.prototype.onExecute = function () {
        var _this = this;
        this.isExecuting(true);

        this._executablePrice.execute(this._parent.notional(), this._parent.dealtCurrency).subscribe(function (trade) {
            return _this.onExecuted(trade);
        }, function (ex) {
            return _this.onExecutionError(ex);
        });
    };

    OneWayPriceViewModel.prototype.onPrice = function (executablePrice) {
        this._executablePrice = executablePrice;

        var formattedPrice = PriceFormatter.getFormattedPrice(executablePrice.rate, executablePrice.parent.currencyPair.ratePrecision, executablePrice.parent.currencyPair.pipsPosition);

        this.bigFigures(formattedPrice.bigFigures);
        this.pips(formattedPrice.pips);
        this.tenthOfPips(formattedPrice.tenthOfPips);
        this.isStale(false);
    };

    OneWayPriceViewModel.prototype.onStalePrice = function () {
        this._executablePrice = null;
        this.bigFigures("");
        this.pips("");
        this.tenthOfPips("");
        this.isStale(true);
    };

    OneWayPriceViewModel.prototype.onExecuted = function (trade) {
        console.log("Trade executed.");
        this._parent.onTrade(trade);
        this.isExecuting(true);
    };

    OneWayPriceViewModel.prototype.onExecutionError = function (ex) {
        // TODO
        console.error(ex);
    };
    return OneWayPriceViewModel;
})();
var FormattedPrice = (function () {
    function FormattedPrice(bigFigures, pips, tenthOfPips) {
        this.bigFigures = bigFigures;
        this.pips = pips;
        this.tenthOfPips = tenthOfPips;
    }
    return FormattedPrice;
})();
var PriceFormatter = (function () {
    function PriceFormatter() {
    }
    PriceFormatter.getFormattedPrice = function (rate, precision, pipsPosition) {
        var rateAsString = rate.toFixed(precision);

        var dotIndex = rateAsString.indexOf(".");

        var bigFigures = rateAsString.substring(0, dotIndex + pipsPosition - 1);
        var pips = rateAsString.substring(dotIndex + pipsPosition - 1, dotIndex + pipsPosition + 1);

        var tenthOfPips = "";

        if (precision > pipsPosition) {
            tenthOfPips = rateAsString.substring(dotIndex + pipsPosition + 1, rateAsString.length);
        }

        return new FormattedPrice(bigFigures, pips, tenthOfPips);
    };

    PriceFormatter.getFormattedSpread = function (spread, precision, pipsPosition) {
        var delta = precision - pipsPosition;
        if (delta > 0) {
            return spread.toFixed(delta);
        }
        return spread.toString();
    };
    return PriceFormatter;
})();
var PriceMovement;
(function (PriceMovement) {
    PriceMovement[PriceMovement["None"] = 0] = "None";
    PriceMovement[PriceMovement["Down"] = 1] = "Down";
    PriceMovement[PriceMovement["Up"] = 2] = "Up";
})(PriceMovement || (PriceMovement = {}));
var SpotTilesViewModel = (function () {
    function SpotTilesViewModel(referenceDataRepository, spotTileViewModelFactory) {
        this._referenceDataRepository = referenceDataRepository;
        this._spotTileViewModelFactory = spotTileViewModelFactory;
        this.spotTiles = ko.observableArray([]);

        this.loadSpotTiles();
    }
    SpotTilesViewModel.prototype.loadSpotTiles = function () {
        var _this = this;
        this._referenceDataRepository.getCurrencyPairs().subscribe(function (currencyPairs) {
            return currencyPairs.forEach(function (cp) {
                return _this.handleCurrencyPairUpdate(cp);
            });
        }, function (ex) {
            return console.error("Failed to get currencies", ex);
        });
    };

    SpotTilesViewModel.prototype.handleCurrencyPairUpdate = function (update) {
        var spotTileViewModel = ko.utils.arrayFirst(this.spotTiles(), function (stvm) {
            return stvm.symbol == update.currencyPair.symbol;
        });
        if (update.updateType == 0 /* Add */) {
            if (spotTileViewModel != null) {
                // we already have a tile for this ccy pair
                return;
            }

            var spotTile = this._spotTileViewModelFactory.create(update.currencyPair);
            this.spotTiles.push(spotTile);
        } else {
            if (spotTileViewModel != null) {
                this.spotTiles.remove(spotTileViewModel);
                spotTileViewModel.dispose();
            }
        }
    };
    return SpotTilesViewModel;
})();
var SpotTileViewModel = (function () {
    function SpotTileViewModel(currencyPair, priceLatencyRecorder) {
        this._priceLatencyRecorder = priceLatencyRecorder;
        this.symbol = currencyPair.baseCurrency + " / " + currencyPair.counterCurrency;
        this._priceSubscription = new Rx.SerialDisposable();
        this._currencyPair = currencyPair;
        this.bid = new OneWayPriceViewModel(this, 1 /* Sell */);
        this.ask = new OneWayPriceViewModel(this, 0 /* Buy */);
        this.notional = ko.observable(1000000);
        this.dealtCurrency = currencyPair.baseCurrency;
        this.spread = ko.observable("");
        this.movement = ko.observable(0 /* None */);
        this.spotDate = ko.observable("SP");
        this.isSubscribing = ko.observable(true);

        this.subscribeForPrices();
    }
    SpotTileViewModel.prototype.dispose = function () {
        if (!this._disposed) {
            this._priceSubscription.dispose();
            this._disposed = true;
        }
    };

    SpotTileViewModel.prototype.executeBid = function () {
        this.bid.onExecute();
    };

    SpotTileViewModel.prototype.executeAsk = function () {
        this.ask.onExecute();
    };

    SpotTileViewModel.prototype.onTrade = function (trade) {
        // TODO
    };

    SpotTileViewModel.prototype.subscribeForPrices = function () {
        var _this = this;
        var subscription = this._currencyPair.prices.subscribe(function (price) {
            return _this.onPrice(price);
        }, function (ex) {
            return console.error(ex);
        });

        this._priceSubscription.setDisposable(subscription);
    };

    SpotTileViewModel.prototype.onPrice = function (price) {
        this.isSubscribing(false);

        if (price.isStale) {
            this.bid.onStalePrice();
            this.ask.onStalePrice();
            this.spread("");
            this._previousRate = null;
            this.movement(0 /* None */);
            this.spotDate("SP");
        } else {
            if (this._previousRate != null) {
                if (price.mid > this._previousRate) {
                    this.movement(2 /* Up */);
                } else if (price.mid < this._previousRate) {
                    this.movement(1 /* Down */);
                } else {
                    this.movement(0 /* None */);
                }
            }

            this._previousRate = price.mid;
            this.bid.onPrice(price.bid);
            this.ask.onPrice(price.ask);

            this.spread(PriceFormatter.getFormattedSpread(price.spread, this._currencyPair.ratePrecision, this._currencyPair.pipsPosition));
            this.spotDate("SP."); //TODO

            this._priceLatencyRecorder.record(price);
        }
    };
    return SpotTileViewModel;
})();
var Direction;
(function (Direction) {
    Direction[Direction["Buy"] = 0] = "Buy";
    Direction[Direction["Sell"] = 1] = "Sell";
})(Direction || (Direction = {}));
var Trade = (function () {
    function Trade(currencyPair, direction, notional, spotRate, tradeStatus, tradeDate, tradeId, traderName, valueDate, dealtCurrency) {
        this.currencyPair = currencyPair;
        this.direction = direction;
        this.notional = notional;
        this.spotRate = spotRate;
        this.tradeStatus = tradeStatus;
        this.tradeDate = tradeDate;
        this.tradeId = tradeId;
        this.traderName = traderName;
        this.valueDate = valueDate;
        this.dealtCurrency = dealtCurrency;
    }
    return Trade;
})();
var TradeFactory = (function () {
    function TradeFactory() {
    }
    TradeFactory.prototype.create = function (trade) {
        return new Trade(trade.CurrencyPair, trade.Direction == 0 /* Buy */ ? 0 /* Buy */ : 1 /* Sell */, trade.Notional, trade.SpotRate, trade.Status == 0 /* Done */ ? 0 /* Done */ : 1 /* Rejected */, trade.TradeDate, trade.TradeId, trade.TraderName, trade.ValueDate, trade.DealtCurrency);
    };
    return TradeFactory;
})();
var TradeStatus;
(function (TradeStatus) {
    TradeStatus[TradeStatus["Done"] = 0] = "Done";
    TradeStatus[TradeStatus["Rejected"] = 1] = "Rejected";
})(TradeStatus || (TradeStatus = {}));
var ExecutablePrice = (function () {
    function ExecutablePrice(direction, rate, executionRepository) {
        this._executionRepository = executionRepository;
        this.direction = direction;
        this.rate = rate;
    }
    ExecutablePrice.prototype.execute = function (notional, dealtCurrency) {
        return this._executionRepository.execute(this, notional, dealtCurrency).take(1);
        // TODO .cacheFirstResult();
    };
    return ExecutablePrice;
})();
var Price = (function () {
    function Price(bid, ask, mid, quoteId, valueDate, currencyPair) {
        this.bid = bid;
        this.ask = ask;
        this.mid = mid;
        this.quoteId = quoteId;
        this.valueDate = valueDate;
        this.currencyPair = currencyPair;
        this.isStale = false;

        bid.parent = this;
        ask.parent = this;

        this.spread = (ask.rate - bid.rate) * Math.pow(10, currencyPair.pipsPosition);
        this._receivedTimestamp = performance.now();
    }
    Object.defineProperty(Price.prototype, "uiProcessingTimeMs", {
        get: function () {
            return this._renderTimestamp - this._receivedTimestamp;
        },
        enumerable: true,
        configurable: true
    });

    Price.prototype.displayedOnUi = function () {
        this._renderTimestamp = performance.now();
    };
    return Price;
})();
var PriceFactory = (function () {
    function PriceFactory(executionRepository) {
        this._executionRepository = executionRepository;
    }
    PriceFactory.prototype.create = function (priceDto, currencyPair) {
        var bid = new ExecutablePrice(1 /* Sell */, priceDto.Bid, this._executionRepository);
        var ask = new ExecutablePrice(0 /* Buy */, priceDto.Ask, this._executionRepository);
        var price = new Price(bid, ask, priceDto.Mid, priceDto.QuoteId, priceDto.ValueDate, currencyPair);

        return price;
    };
    return PriceFactory;
})();
var StalePrice = (function () {
    function StalePrice(currencyPair) {
        this.currencyPair = currencyPair;
        this.isStale = true;
    }
    return StalePrice;
})();
var CurrencyPair = (function () {
    function CurrencyPair(symbol, ratePrecision, pipsPosition, priceRespository) {
        var _this = this;
        this.symbol = symbol;
        this.ratePrecision = ratePrecision;
        this.pipsPosition = pipsPosition;
        this.baseCurrency = symbol.substring(0, 3);
        this.counterCurrency = symbol.substring(3, 6);

        this.prices = Rx.Observable.defer(function () {
            return priceRespository.getPrices(_this);
        }).publish().refCount();
    }
    return CurrencyPair;
})();
var CurrencyPairUpdate = (function () {
    function CurrencyPairUpdate(updateType, currencyPair) {
        this._currencyPair = currencyPair;
        this._updateType = updateType;
    }
    Object.defineProperty(CurrencyPairUpdate.prototype, "updateType", {
        get: function () {
            return this._updateType;
        },
        enumerable: true,
        configurable: true
    });

    Object.defineProperty(CurrencyPairUpdate.prototype, "currencyPair", {
        get: function () {
            return this._currencyPair;
        },
        enumerable: true,
        configurable: true
    });
    return CurrencyPairUpdate;
})();
var CurrencyPairUpdateFactory = (function () {
    function CurrencyPairUpdateFactory(priceRepository) {
        this._priceRepository = priceRepository;
    }
    CurrencyPairUpdateFactory.prototype.create = function (currencyPairUpdate) {
        var cp = new CurrencyPair(currencyPairUpdate.CurrencyPair.Symbol, currencyPairUpdate.CurrencyPair.RatePrecision, currencyPairUpdate.CurrencyPair.PipsPosition, this._priceRepository);

        var update = new CurrencyPairUpdate(currencyPairUpdate.UpdateType == 0 /* Added */ ? 0 /* Add */ : 1 /* Remove */, cp);

        return update;
    };
    return CurrencyPairUpdateFactory;
})();
var UpdateType;
(function (UpdateType) {
    UpdateType[UpdateType["Add"] = 0] = "Add";
    UpdateType[UpdateType["Remove"] = 1] = "Remove";
})(UpdateType || (UpdateType = {}));
var TradeStatusDto;
(function (TradeStatusDto) {
    TradeStatusDto[TradeStatusDto["Done"] = 0] = "Done";
    TradeStatusDto[TradeStatusDto["Rejected"] = 1] = "Rejected";
})(TradeStatusDto || (TradeStatusDto = {}));
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
var ReactiveTrader = (function () {
    function ReactiveTrader() {
    }
    ReactiveTrader.prototype.initialize = function (username, server) {
        this._connection = new Connection(server, username);

        var referenceDataServiceClient = new ReferenceDataServiceClient(this._connection);
        var executionServiceClient = new ExecutionServiceClient(this._connection);
        var blotterServiceClient = new BlotterServiceClient(this._connection);
        var pricingServiceClient = new PricingServiceClient(this._connection);

        var tradeFactory = new TradeFactory();
        var executionRepository = new ExecutionRepository(executionServiceClient, tradeFactory);
        var priceFactory = new PriceFactory(executionRepository);
        var priceRepository = new PriceRepository(pricingServiceClient, priceFactory);
        var currencyPairUpdateFactory = new CurrencyPairUpdateFactory(priceRepository);

        this.tradeRepository = new TradeRepository(blotterServiceClient, tradeFactory);
        this.referenceDataRepository = new ReferenceDataRepository(referenceDataServiceClient, currencyPairUpdateFactory);

        return this._connection.initialize();
    };

    Object.defineProperty(ReactiveTrader.prototype, "connectionStatusStream", {
        get: function () {
            return this._connection.status.publish().refCount();
        },
        enumerable: true,
        configurable: true
    });
    return ReactiveTrader;
})();
var BlotterServiceClient = (function () {
    function BlotterServiceClient(connection) {
        this._connection = connection;
    }
    BlotterServiceClient.prototype.getTradesStream = function () {
        return this.getTradesStreamFromConnection(this._connection.blotterHubProxy);
    };

    BlotterServiceClient.prototype.getTradesStreamFromConnection = function (blotterHub) {
        var _this = this;
        return Rx.Observable.create(function (observer) {
            var tradesSubscription = _this._connection.allTrades.subscribe(observer);

            console.log("Sending blotter subscription...");
            _this._connection.blotterHubProxy.invoke("SubscribeTrades").done(function (_) {
                return console.log("Subscribed to blotter");
            }).fail(function (ex) {
                return observer.onError(ex);
            });

            var unsubscriptionDisposable = Rx.Disposable.create(function () {
                console.log("Sending blotter unsubscription...");

                _this._connection.blotterHubProxy.invoke("UnsubscribeTrades").done(function (_) {
                    return console.log("Unsubscribed from blotter");
                }).fail(function (ex) {
                    return console.error("An error occured while unsubscribing from blotter:" + ex);
                });
            });

            return new Rx.CompositeDisposable([tradesSubscription, unsubscriptionDisposable]);
        }).publish().refCount();
    };
    return BlotterServiceClient;
})();
var ExecutionRepository = (function () {
    function ExecutionRepository(executionServiceClient, tradeFactory) {
        this._tradeFactory = tradeFactory;
        this._executionServiceClient = executionServiceClient;
    }
    ExecutionRepository.prototype.execute = function (executablePrice, notional, dealtCurrency) {
        var _this = this;
        var price = executablePrice.parent;

        var request = new TradeRequestDto();
        request.Direction = executablePrice.direction == 0 /* Buy */ ? 0 /* Buy */ : 1 /* Sell */;
        request.Notional = notional;
        request.QuoteId = price.quoteId;
        request.SpotRate = executablePrice.rate;
        request.Symbol = price.currencyPair.symbol;
        request.ValueDate = price.valueDate;
        request.DealtCurrency = dealtCurrency;

        return this._executionServiceClient.execute(request).timeout(2000).select(function (tradeDto) {
            return _this._tradeFactory.create(tradeDto);
        }).take(1);
        // TODO .CacheFirstResult();
    };
    return ExecutionRepository;
})();
var PriceRepository = (function () {
    function PriceRepository(pricingServiceClient, priceFactory) {
        this._priceFactory = priceFactory;
        this._pricingServiceClient = pricingServiceClient;
    }
    PriceRepository.prototype.getPrices = function (currencyPair) {
        var _this = this;
        return Rx.Observable.defer(function () {
            return _this._pricingServiceClient.getSpotStream(currencyPair.symbol);
        }).select(function (p) {
            return _this._priceFactory.create(p, currencyPair);
        }).catch(Rx.Observable.return(new StalePrice(currencyPair))).repeat().publish().refCount();
    };
    return PriceRepository;
})();
var ReferenceDataRepository = (function () {
    function ReferenceDataRepository(referenceDataServiceClient, currencyPairUpdateFactory) {
        this._currencyPairUpdateFactory = currencyPairUpdateFactory;
        this._referenceDataServiceClient = referenceDataServiceClient;
    }
    ReferenceDataRepository.prototype.getCurrencyPairs = function () {
        var _this = this;
        return Rx.Observable.defer(function () {
            return _this._referenceDataServiceClient.getCurrencyPairUpdates();
        }).where(function (updates) {
            return updates.length > 0;
        }).select(function (updates) {
            return _this.createCurrencyPairUpdates(updates);
        }).catch(function () {
            return Rx.Observable.return([]);
        }).repeat().publish().refCount();
    };

    ReferenceDataRepository.prototype.createCurrencyPairUpdates = function (updates) {
        var result = [];

        for (var i = 0; i < updates.length; i++) {
            result[i] = this._currencyPairUpdateFactory.create(updates[i]);
        }

        return result;
    };
    return ReferenceDataRepository;
})();
var TradeRepository = (function () {
    function TradeRepository(blotterServiceClient, tradeFactory) {
        this._tradeFactory = tradeFactory;
        this._blotterServiceClient = blotterServiceClient;
    }
    TradeRepository.prototype.getTrades = function () {
        var _this = this;
        return Rx.Observable.defer(function () {
            return _this._blotterServiceClient.getTradesStream();
        }).select(function (trades) {
            return _this.createTrades(trades);
        }).catch(function () {
            return Rx.Observable.return([]);
        }).repeat().publish().refCount();
    };

    TradeRepository.prototype.createTrades = function (trades) {
        var result = [];

        for (var i = 0; i < trades.length; i++) {
            result[i] = this._tradeFactory.create(trades[i]);
        }

        return result;
    };
    return TradeRepository;
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
        }).take(1).timeout(2000);
        // TODO cachefirstresult
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
            subscriptionRequest.CurrencyPair = currencyPair;

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
        }).publish().refCount();
    };
    return PricingServiceClient;
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
        }).publish().refCount();
    };
    return ReferenceDataServiceClient;
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

    Object.defineProperty(Connection.prototype, "allTrades", {
        get: function () {
            return this._allTrades;
        },
        enumerable: true,
        configurable: true
    });

    Connection.prototype.installListeners = function () {
        var _this = this;
        this._allPrices = new Rx.Subject();
        this._currencyPairUpdates = new Rx.Subject();
        this._allTrades = new Rx.Subject();

        this._pricingHubProxy.on("OnNewPrice", function (price) {
            return _this._allPrices.onNext(price);
        });
        this._referenceDataHubProxy.on("OnCurrencyPairUpdate", function (currencyPairs) {
            return _this._currencyPairUpdates.onNext(currencyPairs);
        });
        this._blotterHubProxy.on("OnNewTrade", function (trades) {
            return _this._allTrades.onNext(trades);
        });
    };
    return Connection;
})();
var SpotTileViewModelFactory = (function () {
    function SpotTileViewModelFactory(priceLatencyRecorder) {
        this._priceLatencyRecorder = priceLatencyRecorder;
    }
    SpotTileViewModelFactory.prototype.create = function (currencyPair) {
        return new SpotTileViewModel(currencyPair, this._priceLatencyRecorder);
    };
    return SpotTileViewModelFactory;
})();
//# sourceMappingURL=reactiveTrader.js.map

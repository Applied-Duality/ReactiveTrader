class PriceLatencyRecorder implements IPriceLatencyRecorder {
    private _count: number;
    private _maxLatency: IPriceLatency;

    record(price: IPrice) {
        var priceLatency = <IPriceLatency><any>price;
        if (priceLatency != null) {
            priceLatency.displayedOnUi();

            this._count++;
            if (this._maxLatency == null || priceLatency.uiProcessingTimeMs > this._maxLatency.uiProcessingTimeMs) {
                this._maxLatency = priceLatency;
            }
        }
    }

    getMaxLatencyAndReset(): MaxLatency {
        var result = new MaxLatency(this._count, this._maxLatency);
        this._count = 0;
        this._maxLatency = null;
        return result;
    }
} 
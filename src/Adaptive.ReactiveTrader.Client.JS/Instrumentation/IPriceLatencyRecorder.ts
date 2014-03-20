 interface IPriceLatencyRecorder {
     record(price: IPrice);
     getMaxLatencyAndReset(): MaxLatency;
 }

 
using System;

namespace Adaptive.ReactiveTrader.Client.Instrumentation
{
    public class Statistics
    {
        public long UiLatencyMax { get; set; }
        public long ServerLatencyMax { get; set; }
        public long UiLatencyStdDev { get; set; }
        public long ServerLatencyStdDev { get; set; }

        public long ServerToUiLatencyMax { get { return UiLatencyMax + ServerLatencyMax; } }
        
        public long Count { get; set; }

        public TimeSpan ProcessTime { get; set; }
    }
}
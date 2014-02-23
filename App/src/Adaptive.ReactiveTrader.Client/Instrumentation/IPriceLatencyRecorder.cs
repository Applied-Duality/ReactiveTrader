using System;

namespace Adaptive.ReactiveTrader.Client.Instrumentation
{
    public interface IPriceLatencyRecorder
    {
        void RecordProcessingTime(TimeSpan elapsed);
        Tuple<TimeSpan, long> GetCurrentAndReset();
    }
}
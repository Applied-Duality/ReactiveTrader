using System;

namespace Adaptive.ReactiveTrader.Client.Instrumentation
{
    class PriceLatencyRecorder : IPriceLatencyRecorder
    {
        private TimeSpan _currentWorst = TimeSpan.Zero;
        private long _count;

        public void RecordProcessingTime(TimeSpan elapsed)
        {
            _count++;
            if (elapsed > _currentWorst)
            {
                _currentWorst = elapsed;
            }
        }

        public Tuple<TimeSpan, long> GetCurrentAndReset()
        {
            var value = _currentWorst;
            var count = _count;
            _currentWorst = TimeSpan.Zero;
            _count = 0;
            return new Tuple<TimeSpan, long>(value, count);
        }
    }
}
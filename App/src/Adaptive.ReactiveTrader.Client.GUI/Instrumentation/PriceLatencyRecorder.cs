using System;
using Adaptive.ReactiveTrader.Client.Domain.Models.Pricing;

namespace Adaptive.ReactiveTrader.Client.Instrumentation
{
    class PriceLatencyRecorder : IPriceLatencyRecorder
    {
        private long _count;
        private IPriceLatency _maxLatency;

        public void Record(IPrice price)
        {
            var priceLatency = price as IPriceLatency;
            if (priceLatency != null)
            {
                priceLatency.DisplayedOnUi();

                _count++;
                if (_maxLatency == null || priceLatency.TotalLatencyMs > _maxLatency.TotalLatencyMs)
                {
                    _maxLatency = priceLatency;
                }
            }
        }

        public Tuple<IPriceLatency, long> GetMaxLatencyAndReset()
        {
            var value = _maxLatency;
            var count = _count;
            _maxLatency = null;
            _count = 0;
            return new Tuple<IPriceLatency, long>(value, count);
        }
    }
}
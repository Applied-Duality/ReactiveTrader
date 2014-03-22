using System;
using Adaptive.ReactiveTrader.Client.Domain.Models.Pricing;

namespace Adaptive.ReactiveTrader.Client.Instrumentation
{
    public interface IPriceLatencyRecorder
    {
        void Record(IPrice price);
        Statistics CalculateAndReset();
    }
}
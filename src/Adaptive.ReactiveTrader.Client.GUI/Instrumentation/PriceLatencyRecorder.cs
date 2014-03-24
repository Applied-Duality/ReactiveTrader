using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Adaptive.ReactiveTrader.Client.Domain.Models.Pricing;

namespace Adaptive.ReactiveTrader.Client.Instrumentation
{
    class PriceLatencyRecorder : IPriceLatencyRecorder
    {
        private readonly IList<IPriceLatency> _prices = new List<IPriceLatency>(8192);
        private readonly Process _currentProcess;
        private TimeSpan _lastProcessTime;

        public PriceLatencyRecorder()
        {
            _currentProcess = Process.GetCurrentProcess();
            _lastProcessTime = _currentProcess.UserProcessorTime;
            
        }
        public void Record(IPrice price)
        {
            var priceLatency = price as IPriceLatency;
            if (priceLatency != null)
            {
                priceLatency.DisplayedOnUi();
                _prices.Add(priceLatency);
            }
        }

        public Statistics CalculateAndReset()
        {
            if (!_prices.Any())
                return null;

            var stats = new Statistics();
            
            stats.Count = _prices.Count;
            stats.ServerLatencyMax = (long) _prices.MaxBy(pl => pl.ServerToClientMs).FirstOrDefault().ServerToClientMs;
            stats.ServerLatencyStdDev = (long) CalculateStdDev(_prices.Select(pl => pl.ServerToClientMs));
            
            stats.UiLatencyMax = (long) _prices.MaxBy(pl => pl.UiProcessingTimeMs).FirstOrDefault().UiProcessingTimeMs;
            stats.UiLatencyStdDev = (long) CalculateStdDev(_prices.Select(pl => pl.UiProcessingTimeMs));

            var currentProcessTime = _currentProcess.UserProcessorTime;
            
            stats.ProcessTime = currentProcessTime.Subtract(_lastProcessTime);
            
            _lastProcessTime = currentProcessTime;
            
            _prices.Clear();
            
            return stats;
        }

        private double CalculateStdDev(IEnumerable<double> values)
        {
            double ret = 0;
            if (values.Any())
            {
                //Compute the Average      
                double avg = values.Average();

                //Perform the Sum of (value-avg)^2
                
                double sum = values.Sum(d => Math.Pow(d - avg, 2));
                //Put it all together      
                ret = Math.Sqrt((sum) / (values.Count() - 1));
            }
            return ret;
        }
    }
}
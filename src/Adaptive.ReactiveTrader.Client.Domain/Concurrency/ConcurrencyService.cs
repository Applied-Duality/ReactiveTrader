using System.Reactive.Concurrency;

namespace Adaptive.ReactiveTrader.Client.Domain.Concurrency
{
    internal sealed class ConcurrencyService : IConcurrencyService
    {
        public IScheduler ThreadPool
        {
            get { return ThreadPoolScheduler.Instance; }
        }
    }
}
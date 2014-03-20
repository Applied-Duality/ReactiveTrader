using System.Reactive.Concurrency;

namespace Adaptive.ReactiveTrader.Client.Concurrency
{
    public sealed class ConcurrencyService : IConcurrencyService
    {
        private readonly PeriodicBatchScheduler _scheduler;

        public ConcurrencyService()
        {
            _scheduler = new PeriodicBatchScheduler(DispatcherScheduler.Current);
        }
        public IScheduler Dispatcher
        {
            get { return DispatcherScheduler.Current; }
        }

        public IScheduler ThreadPool
        {
            get { return ThreadPoolScheduler.Instance; }
        }

        public IScheduler DispatcherPeriodic
        {
            get { return _scheduler; }
        }
    }
}
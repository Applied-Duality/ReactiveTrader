using System.Reactive.Concurrency;

namespace Adaptive.ReactiveTrader.Client.Concurrency
{
    public sealed class SchedulerProvider : ISchedulerProvider
    {
        public IScheduler CurrentThread
        {
            get { return Scheduler.CurrentThread; }
        }

        public IScheduler Dispatcher
        {
            get { return DispatcherScheduler.Current; }
        }

        public IScheduler Immediate
        {
            get { return Scheduler.Immediate; }
        }

        public IScheduler NewThread
        {
            get { return NewThreadScheduler.Default; }
        }

        public IScheduler ThreadPool
        {
            get { return ThreadPoolScheduler.Instance; }
        }

        public IScheduler TaskPool
        {
            get { return TaskPoolScheduler.Default; }
        }
    }
}
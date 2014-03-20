using System.Reactive.Concurrency;

namespace Adaptive.ReactiveTrader.Client.Concurrency
{
    public interface IConcurrencyService
    {
        IScheduler Dispatcher { get; }
        IScheduler DispatcherPeriodic { get; }
        IScheduler ThreadPool { get; }
    }
}
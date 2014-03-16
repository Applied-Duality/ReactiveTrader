using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;

namespace Adaptive.ReactiveTrader.Client.Concurrency
{
    public class PeriodicBatchScheduler : IScheduler
    {
        private readonly IScheduler _scheduler;
        private readonly List<Action> _queuedWork = new List<Action>();
        private readonly object _queuedWorkLock = new object();
        private IDisposable _periodSchedulerDisposable;

        public PeriodicBatchScheduler(IScheduler scheduler)
        {
            _scheduler = scheduler;
        }

        public IDisposable Schedule<TState>(TState state, Func<IScheduler, TState, IDisposable> action)
        {
            Action work = () => action(_scheduler, state);

            lock (_queuedWorkLock)
            {
                _queuedWork.Add(work);
                if (_queuedWork.Count == 1)
                {
                    _periodSchedulerDisposable = _scheduler.SchedulePeriodic<object>(null, TimeSpan.FromMilliseconds(50), _ => Run());
                }
            }

            return Disposable.Create(() =>
            {
                lock (_queuedWorkLock)
                {
                    _queuedWork.Remove(work);
                    if (_queuedWork.Count == 0)
                    {
                        _periodSchedulerDisposable.Dispose();
                    }
                }
            });
        }

        public IDisposable Schedule<TState>(TState state, TimeSpan dueTime, Func<IScheduler, TState, IDisposable> action)
        {
            throw new NotSupportedException();
        }

        public IDisposable Schedule<TState>(TState state, DateTimeOffset dueTime, Func<IScheduler, TState, IDisposable> action)
        {
            throw new NotSupportedException();
        }

        public DateTimeOffset Now { get { return _scheduler.Now; } }

        public void Run()
        {
            Action[] work;

            lock (_queuedWorkLock)
            {
                work = _queuedWork.ToArray();
            }

            Array.ForEach(work, item => item());
        }
    }
}
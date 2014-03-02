using System;
using System.Reactive;
using System.Reactive.Concurrency;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;

namespace Adaptive.ReactiveTrader.Shared.Extensions
{
    public static class ObservableExtensions
    {
        public static IObservable<T> AutoConnect<T>(this IConnectableObservable<T> connectable)
        {
            var connected = 0;
            return Observable.Create<T>(observer =>
            {
                var subscription = connectable.Subscribe(observer);
                if (Interlocked.CompareExchange(ref connected, 1, 0) == 0)
                {
                    connectable.Connect();
                }
                return subscription;
            }).AsObservable();
        }

        public static IObservable<T> CacheFirstResult<T>(this IObservable<T> observable)
        {
            return observable.Take(1).PublishLast().AutoConnect();
        }

        public static IObservable<TSource> TakeUntilInclusive<TSource>(this IObservable<TSource> source, Func<TSource, Boolean> predicate)
        {
            return Observable.Create<TSource>(
                observer => source.Subscribe(
                  item =>
                  {
                      observer.OnNext(item);
                      if (predicate(item))
                          observer.OnCompleted();
                  },
                  observer.OnError,
                  observer.OnCompleted
                )
              );
        }

        public static IObservable<T> OnSubscribe<T>(this IObservable<T> source, Action onSubscribe)
        {
            return Observable.Create<T>(observer =>
            {
                onSubscribe();
                return source.Subscribe(observer);
            });
        }

        public static IObservable<T> ObserveLatestOn<T>(this IObservable<T> source, IScheduler scheduler)
        {
            return Observable.Create<T>(observer =>
            {
                var gate = new object();
                bool active = false;
                var cancelable = new MultipleAssignmentDisposable();
                var disposable = source.Materialize().Subscribe(thisNotification =>
                {
                    bool wasNotAlreadyActive;
                    Notification<T> outsideNotification;
                    lock (gate)
                    {
                        wasNotAlreadyActive = !active;
                        active = true;
                        outsideNotification = thisNotification;
                    }

                    if (wasNotAlreadyActive)
                    {
                        cancelable.Disposable = scheduler.Schedule(self =>
                        {
                            Notification<T> localNotification;
                            lock (gate)
                            {
                                localNotification = outsideNotification;
                                outsideNotification = null;
                            }
                            localNotification.Accept(observer);
                            bool hasPendingNotification;
                            lock (gate)
                            {
                                hasPendingNotification = active = (outsideNotification != null);
                            }
                            if (hasPendingNotification)
                            {
                                self();
                            }
                        });
                    }
                });
                return new CompositeDisposable(disposable, cancelable);
            });
        }

        /// <summary>
        /// Applies a conflation algorithm to an observable stream. 
        /// Anytime the stream OnNext twice below minimumUpdatePeriod, the second update gets delayed to respect the minimumUpdatePeriod
        /// If more than 2 update happen, only the last update is pushed
        /// Updates are pushed and rescheduled using the provided scheduler
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">stream</param>
        /// <param name="minimumUpdatePeriod">minimum delay between 2 updates</param>
        /// <param name="scheduler">to be used to publish updates and schedule delayed updates</param>
        /// <returns></returns>
        public static IObservable<T> Conflate<T>(this IObservable<T> source, TimeSpan minimumUpdatePeriod, IScheduler scheduler)
        {
            return Observable.Create<T>(observer =>
            {
                // indicate when the last update was published
                var lastUpdateTime = DateTimeOffset.MinValue;
                // indicate if an update is currently scheduled
                var updateScheduled = new MultipleAssignmentDisposable();
                // indicate if completion has been requested (we can't complete immediatly if an update is in flight)
                var completionRequested = false;
                var gate = new object();

                var sourceSubscription =
                    source
                        .ObserveOn(scheduler)
                        .Subscribe(
                            x =>
                            {
                                var currentUpdateTime = scheduler.Now;

                                bool scheduleRequired;
                                lock (gate)
                                {
                                    scheduleRequired = currentUpdateTime - lastUpdateTime < minimumUpdatePeriod;
                                    if (scheduleRequired && updateScheduled.Disposable != null)
                                    {
                                        updateScheduled.Disposable.Dispose();
                                        updateScheduled.Disposable = null;
                                    }
                                }

                                if (scheduleRequired)
                                {
                                    updateScheduled.Disposable = scheduler.Schedule(lastUpdateTime + minimumUpdatePeriod, () =>
                                    {
                                        observer.OnNext(x);

                                        lock (gate)
                                        {
                                            lastUpdateTime = scheduler.Now;
                                            updateScheduled.Disposable = null;
                                            if (completionRequested)
                                            {
                                                observer.OnCompleted();
                                            }
                                        }
                                    });
                                }
                                else
                                {
                                    observer.OnNext(x);
                                    lock (gate)
                                    {
                                        lastUpdateTime = scheduler.Now;
                                    }
                                }
                            },
                            observer.OnError,
                            () =>
                            {
                                // if we have scheduled an update we need to complete once the update has been published
                                if (updateScheduled.Disposable != null)
                                {
                                    lock (gate)
                                    {
                                        completionRequested = true;                                        
                                    }
                                }
                                else
                                {
                                    observer.OnCompleted();
                                }
                            });

                return new CompositeDisposable { sourceSubscription };
            });
        }
    }
}

using System;
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
    }
}

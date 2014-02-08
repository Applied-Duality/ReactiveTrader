using System;

namespace Adaptive.ReactiveTrader.Client.Transport
{
    public interface IConnectionProvider
    {
        IObservable<IConnection> GetActiveConnection();
    }
}
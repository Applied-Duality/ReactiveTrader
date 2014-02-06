using System;
using System.Reactive;

namespace Adaptive.ReactiveTrader.Client.Transport
{
    public interface ITransport
    {
        IObservable<Unit> Initialize(string address, string username);
        IObservable<TransportStatus> Status { get; }
    }
}
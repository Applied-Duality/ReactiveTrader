using System;
using System.Reactive;
using Microsoft.AspNet.SignalR.Client;

namespace Adaptive.ReactiveTrader.Client.Transport
{
    public interface IConnection
    {
        IObservable<ConnectionStatus> Status { get; }
        IHubProxy GetProxy(string name);
        IObservable<Unit> Initialize();
    }
}
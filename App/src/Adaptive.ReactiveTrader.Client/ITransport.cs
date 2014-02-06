using System;
using System.Reactive;
using Microsoft.AspNet.SignalR.Client;

namespace Adaptive.ReactiveTrader.Client
{
    public interface ITransport
    {
        IObservable<Unit> Initialize(string address, string userName);
        IObservable<TransportStatus> TransportStatuses { get; } 
        IHubProxy HubProxy { get; }
    }
}
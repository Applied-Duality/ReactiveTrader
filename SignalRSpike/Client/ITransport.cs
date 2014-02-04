using System;
using System.Reactive;
using Microsoft.AspNet.SignalR.Client;

namespace Client
{
    public interface ITransport
    {
        IObservable<Unit> Initialize(string address, string userName);
        IObservable<TransportStatus> TransportStatuses { get; } 
        IHubProxy HubProxy { get; }
    }
}
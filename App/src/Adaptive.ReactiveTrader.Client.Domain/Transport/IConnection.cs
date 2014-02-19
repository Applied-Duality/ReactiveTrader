using System;
using System.Reactive;
using Microsoft.AspNet.SignalR.Client;

namespace Adaptive.ReactiveTrader.Client.Domain.Transport
{
    internal interface IConnection
    {
        IObservable<ConnectionStatus> Status { get; }
        IObservable<bool> IsConnected { get; }
        IObservable<Unit> Initialize();
        IHubProxy ReferenceDataHubProxy { get; }
        IHubProxy PricingHubProxy { get; }
        IHubProxy ExecutionHubProxy { get; }
        IHubProxy BlotterHubProxy { get; }
    }
 }
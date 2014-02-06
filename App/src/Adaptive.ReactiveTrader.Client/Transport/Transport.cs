using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Adaptive.ReactiveTrader.Contracts;
using log4net;
using Microsoft.AspNet.SignalR.Client;

namespace Adaptive.ReactiveTrader.Client.Transport
{
    public class Transport : ITransport
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Transport));

        private readonly ISubject<TransportStatus> _transportStatuses = new ReplaySubject<TransportStatus>(1);

        public IObservable<Unit> Initialize(string address, string userName)
        {
            return Observable.Create<Unit>(async observer =>
            {
                _transportStatuses.OnNext(TransportStatus.Connecting);

                var hubConnection = new HubConnection(address);
                hubConnection.Headers.Add(ServiceConstants.Server.UsernameHeader, userName);

                PricingHubProxy = hubConnection.CreateHubProxy(ServiceConstants.Server.PricingHub);
                BlotterHubProxy = hubConnection.CreateHubProxy(ServiceConstants.Server.BlotterHub);
                ReferenceDataHubProxy = hubConnection.CreateHubProxy(ServiceConstants.Server.ReferenceDataHub);
                ExecutionHubProxy = hubConnection.CreateHubProxy(ServiceConstants.Server.ExecutionHub);

                Log.InfoFormat("Connecting to server {0} with user {1}...", address, userName);

                // http://www.asp.net/signalr/overview/signalr-20/hubs-api/handling-connection-lifetime-events#timeoutkeepalive
                // TODO unsubscribe events 
                hubConnection.Closed += () =>
                {
                    Log.WarnFormat("SignalR connection closed.");
                    _transportStatuses.OnNext(TransportStatus.Closed);
                    _transportStatuses.OnCompleted();
                };
                hubConnection.ConnectionSlow += () =>
                {
                    Log.WarnFormat("SignalR has detected a slow connection");
                    _transportStatuses.OnNext(TransportStatus.ConnectionSlow);
                };
                hubConnection.Error += exception => Log.Error("An error occured in the transport.", exception);
                hubConnection.Reconnected += () =>
                {
                    Log.InfoFormat("SignalR transport reconnected.");
                    _transportStatuses.OnNext(TransportStatus.Reconnected);
                };
                hubConnection.Reconnecting += () =>
                {
                    Log.InfoFormat("SignalR transport is reconnecting...");
                    _transportStatuses.OnNext(TransportStatus.Reconnecting);
                };

                try
                {
                    await hubConnection.Start();
                     _transportStatuses.OnNext(TransportStatus.Connected);
                    observer.OnNext(new Unit());
                }
                catch (Exception e)
                {
                    Log.Error("An error occured when starting transport", e);
                    observer.OnError(e); 
                }

                return Disposable.Create(() =>
                {
                    try
                    {
                        Log.Info("Stoping transport...");
                        hubConnection.Stop();
                        Log.Info("Transport stopped");
                    }
                    catch (Exception e)
                    {
                        // we must never throw in a disposable
                        Log.Error("An error occured while stoping transport", e);
                    }
                });
            });
        }

        public IObservable<TransportStatus> TransportStatuses
        {
            get { return _transportStatuses; }
        }

        public IHubProxy PricingHubProxy { get; private set; }
        public IHubProxy BlotterHubProxy { get; private set; }
        public IHubProxy ExecutionHubProxy { get; private set; }
        public IHubProxy ReferenceDataHubProxy { get; private set; }
    }
}
using System;
using System.Collections.Concurrent;
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
        private HubConnection _hubConnection;
        private readonly ConcurrentDictionary<string, IHubProxy> _proxies = new ConcurrentDictionary<string, IHubProxy>(); 

        public IObservable<Unit> Initialize(string address, string userName)
        {
            return Observable.Create<Unit>(async observer =>
            {
                _transportStatuses.OnNext(TransportStatus.Connecting);

                _hubConnection = new HubConnection(address);
                _hubConnection.Headers.Add(ServiceConstants.Server.UsernameHeader, userName);

                Log.InfoFormat("Connecting to server {0} with user {1}...", address, userName);

                // http://www.asp.net/signalr/overview/signalr-20/hubs-api/handling-connection-lifetime-events#timeoutkeepalive
                // TODO unsubscribe events 
                _hubConnection.Closed += () =>
                {
                    Log.WarnFormat("SignalR connection closed.");
                    _transportStatuses.OnNext(TransportStatus.Closed);
                    _transportStatuses.OnCompleted();
                };
                _hubConnection.ConnectionSlow += () =>
                {
                    Log.WarnFormat("SignalR has detected a slow connection");
                    _transportStatuses.OnNext(TransportStatus.ConnectionSlow);
                };
                _hubConnection.Error += exception => Log.Error("An error occured in the transport.", exception);
                _hubConnection.Reconnected += () =>
                {
                    Log.InfoFormat("SignalR transport reconnected.");
                    _transportStatuses.OnNext(TransportStatus.Reconnected);
                };
                _hubConnection.Reconnecting += () =>
                {
                    Log.InfoFormat("SignalR transport is reconnecting...");
                    _transportStatuses.OnNext(TransportStatus.Reconnecting);
                };

                try
                {
                    await _hubConnection.Start();
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
                        _hubConnection.Stop();
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
 
        public IHubProxy GetProxy(string hubName)
        {
            return _proxies.GetOrAdd(hubName, s => _hubConnection.CreateHubProxy(s));
        }
    }
}
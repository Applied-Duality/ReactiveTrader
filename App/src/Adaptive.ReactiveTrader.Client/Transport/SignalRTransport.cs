using System;
using System.Collections.Concurrent;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Adaptive.ReactiveTrader.Contracts;
using Adaptive.ReactiveTrader.Contracts.Extensions;
using log4net;
using Microsoft.AspNet.SignalR.Client;

namespace Adaptive.ReactiveTrader.Client.Transport
{
    public class SignalRTransport : ISignalRTransport
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(SignalRTransport));

        private HubConnection _hubConnection;
        private readonly ConcurrentDictionary<string, IHubProxy> _proxies = new ConcurrentDictionary<string, IHubProxy>();
        private readonly ISubject<TransportStatus> _status = new BehaviorSubject<TransportStatus>(TransportStatus.Uninitialized);
        private bool _initialized;

        public IObservable<Unit> Initialize(string address, string username)
        {
            if(_initialized) throw new InvalidOperationException("Transport has already been initialized");

            _initialized = true;

            return Observable.Create<Unit>(async observer =>
            {
                _status.OnNext(TransportStatus.Connecting);
                _hubConnection = new HubConnection(address);

                CreateStatus().Subscribe(_status.OnNext);

                _hubConnection.Headers.Add(ServiceConstants.Server.UsernameHeader, username);

                Log.InfoFormat("Connecting to server {0} with user {1}...", address, username);

                _hubConnection.Error += exception => Log.Error("An error occured in the transport.", exception);

                try
                {
                    await _hubConnection.Start();
                    _status.OnNext(TransportStatus.Connected);
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
                        Log.Info("SignalRTransport stopped");
                    }
                    catch (Exception e)
                    {
                        // we must never throw in a disposable
                        Log.Error("An error occured while stoping transport", e);
                    }
                });
            });
        }

        private IObservable<TransportStatus> CreateStatus()
        {
            var closed = Observable.FromEvent(h => _hubConnection.Closed += h, h => _hubConnection.Closed -= h).Select(_ => TransportStatus.Closed);
            var connectionSlow = Observable.FromEvent(h => _hubConnection.ConnectionSlow += h, h => _hubConnection.ConnectionSlow -= h).Select(_ => TransportStatus.ConnectionSlow);
            var reconnected = Observable.FromEvent(h => _hubConnection.Reconnected += h, h => _hubConnection.Reconnected -= h).Select(_ => TransportStatus.Reconnected);
            var reconnecting = Observable.FromEvent(h => _hubConnection.Reconnecting += h, h => _hubConnection.Reconnecting -= h).Select(_ => TransportStatus.Reconnecting);
            return Observable.Merge(closed, connectionSlow, reconnected, reconnecting);
        }

        public IObservable<TransportStatus> Status
        {
            get { return _status; }
        }

        IHubProxy ISignalRTransport.GetProxy(string hubName)
        {
            return _proxies.GetOrAdd(hubName, s => _hubConnection.CreateHubProxy(s));
        }
    }
}
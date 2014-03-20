using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Adaptive.ReactiveTrader.Shared;
using Adaptive.ReactiveTrader.Shared.Extensions;
using log4net;
using Microsoft.AspNet.SignalR.Client;

namespace Adaptive.ReactiveTrader.Client.Domain.Transport
{
    /// <summary>
    /// This represents a single SignalR connection.
    /// The <see cref="ConnectionProvider"/> creates connections and is responsible for creating new one when a connection is closed.
    /// </summary>
    internal class Connection : IConnection
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Connection));

        private readonly ISubject<ConnectionInfo> _statusStream;
        private readonly HubConnection _hubConnection;

        private bool _initialized;

        public Connection(string address, string username)
        {
            _statusStream = new BehaviorSubject<ConnectionInfo>(new ConnectionInfo(ConnectionStatus.Uninitialized, address));
            Address = address;
            _hubConnection = new HubConnection(address);
            _hubConnection.Headers.Add(ServiceConstants.Server.UsernameHeader, username);
            CreateStatus().Subscribe(
                s => _statusStream.OnNext(new ConnectionInfo(s, address)),
                _statusStream.OnError,
                _statusStream.OnCompleted);
            _hubConnection.Error += exception => Log.Error("There was a connection error with " + address, exception);

            BlotterHubProxy = _hubConnection.CreateHubProxy(ServiceConstants.Server.BlotterHub);
            ExecutionHubProxy = _hubConnection.CreateHubProxy(ServiceConstants.Server.ExecutionHub);
            PricingHubProxy = _hubConnection.CreateHubProxy(ServiceConstants.Server.PricingHub);
            ReferenceDataHubProxy = _hubConnection.CreateHubProxy(ServiceConstants.Server.ReferenceDataHub);
        }

        public IObservable<Unit> Initialize()
        {
            if (_initialized)
            {
                throw new InvalidOperationException("Connection has already been initialized");
            }
            _initialized = true;

            return Observable.Create<Unit>(async observer =>
            {
                _statusStream.OnNext(new ConnectionInfo(ConnectionStatus.Connecting, Address)); 

                try
                {
                    Log.InfoFormat("Connecting to {0}", Address);
                    await _hubConnection.Start();
                    _statusStream.OnNext(new ConnectionInfo(ConnectionStatus.Connected, Address));
                    observer.OnNext(Unit.Default);
                }
                catch (Exception e)
                {
                    Log.Error("An error occured when starting SignalR connection", e);
                    observer.OnError(e);
                }

                return Disposable.Create(() =>
                {
                    try
                    {
                        Log.Info("Stoping connection...");
                        _hubConnection.Stop();
                        Log.Info("Connection stopped");
                    }
                    catch (Exception e)
                    {
                        // we must never throw in a disposable
                        Log.Error("An error occured while stoping connection", e);
                    }
                });
            })
            .Publish()
            .RefCount();
        } 

        private IObservable<ConnectionStatus> CreateStatus()
        {
            var closed = Observable.FromEvent(h => _hubConnection.Closed += h, h => _hubConnection.Closed -= h).Select(_ => ConnectionStatus.Closed);
            var connectionSlow = Observable.FromEvent(h => _hubConnection.ConnectionSlow += h, h => _hubConnection.ConnectionSlow -= h).Select(_ => ConnectionStatus.ConnectionSlow);
            var reconnected = Observable.FromEvent(h => _hubConnection.Reconnected += h, h => _hubConnection.Reconnected -= h).Select(_ => ConnectionStatus.Reconnected);
            var reconnecting = Observable.FromEvent(h => _hubConnection.Reconnecting += h, h => _hubConnection.Reconnecting -= h).Select(_ => ConnectionStatus.Reconnecting);
            return Observable.Merge(closed, connectionSlow, reconnected, reconnecting)
                .TakeUntilInclusive(status => status == ConnectionStatus.Closed); // complete when the connection is closed (it's terminal, SignalR will not attempt to reconnect anymore)
        }

        public IObservable<ConnectionInfo> StatusStream
        {
            get { return _statusStream; }
        }

        public string Address { get; private set; }

        public IHubProxy ReferenceDataHubProxy { get; private set; }
        public IHubProxy PricingHubProxy { get; private set; }
        public IHubProxy ExecutionHubProxy { get; private set; }
        public IHubProxy BlotterHubProxy { get; private set; }

        public override string ToString()
        {
            return string.Format("Address: {0}", Address);
        }
    }
}
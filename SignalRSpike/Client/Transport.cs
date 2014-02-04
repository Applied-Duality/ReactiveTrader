using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Dto;
using log4net;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Client;

namespace Client
{
    public class Transport : ITransport
    {
        private IHubProxy _tradingServiceHub;

        private static readonly ILog Log = LogManager.GetLogger(typeof(Transport));

        private readonly ISubject<TransportStatus> _transportStatuses = new ReplaySubject<TransportStatus>(1);

        public IObservable<Unit> Initialize(string address, string userName)
        {
            return Observable.Create<Unit>(async observer =>
            {
                _transportStatuses.OnNext(TransportStatus.Connecting);
                SetupTimeouts();

                var hubConnection = new HubConnection(address);
                hubConnection.Headers.Add(ServiceConstants.Server.UsernameHeader, userName);
                _tradingServiceHub = hubConnection.CreateHubProxy(ServiceConstants.Server.TradingServiceHub);

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

        private static void SetupTimeouts()
        {
            // Make long polling connections wait a maximum of 110 seconds for a
            // response. When that time expires, trigger a timeout command and
            // make the client reconnect.
            GlobalHost.Configuration.ConnectionTimeout = TimeSpan.FromSeconds(110);

            // Wait a maximum of 10 seconds after a transport connection is lost
            // before raising the Disconnected event to terminate the SignalR connection.
            GlobalHost.Configuration.DisconnectTimeout = TimeSpan.FromSeconds(10);

            // For transports other than long polling, send a keepalive packet every
            // 3 seconds. 
            // This value must be no more than 1/3 of the DisconnectTimeout value.
            GlobalHost.Configuration.KeepAlive = TimeSpan.FromSeconds(3);
        }

        public IHubProxy HubProxy
        {
            get { return _tradingServiceHub; }
        }
    }
}
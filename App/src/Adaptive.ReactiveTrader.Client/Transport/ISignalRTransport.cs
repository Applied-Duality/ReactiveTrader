using Microsoft.AspNet.SignalR.Client;

namespace Adaptive.ReactiveTrader.Client.Transport
{
    internal interface ISignalRTransport : ITransport
    {
        IHubProxy GetProxy(string hubName);
    }
}
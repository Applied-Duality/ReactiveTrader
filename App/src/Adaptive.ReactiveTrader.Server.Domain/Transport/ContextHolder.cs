using Microsoft.AspNet.SignalR.Hubs;

namespace Adaptive.ReactiveTrader.Server.Transport
{
    public class ContextHolder : IContextHolder
    {
        public IHubCallerConnectionContext Context { get; set; }
        public IHubCallerConnectionContext BlotterHubClients { get; set; }
    }
}
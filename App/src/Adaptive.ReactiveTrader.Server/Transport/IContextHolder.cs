using Microsoft.AspNet.SignalR.Hubs;

namespace Adaptive.ReactiveTrader.Server.Transport
{
    public interface IContextHolder
    {
        IHubCallerConnectionContext Context { get; set; }
    }
}
using Microsoft.AspNet.SignalR.Hubs;

namespace Adaptive.ReactiveTrader.Server.SignalR
{
    public interface IContextHolder
    {
        IHubCallerConnectionContext Context { get; set; }
    }
}
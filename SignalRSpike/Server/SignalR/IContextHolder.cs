using Microsoft.AspNet.SignalR.Hubs;

namespace Server.SignalR
{
    public interface IContextHolder
    {
        IHubCallerConnectionContext Context { get; set; }
    }
}
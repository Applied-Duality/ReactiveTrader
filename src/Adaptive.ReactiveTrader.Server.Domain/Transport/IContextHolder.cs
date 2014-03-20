using Microsoft.AspNet.SignalR.Hubs;

namespace Adaptive.ReactiveTrader.Server.Transport
{
    public interface IContextHolder
    {
        IHubCallerConnectionContext PricingHubClient { get; set; }
        IHubCallerConnectionContext BlotterHubClients { get; set; }
        IHubCallerConnectionContext ReferenceDataHubClients { get; set; }
    }
}
using Microsoft.AspNet.SignalR.Hubs;

namespace Adaptive.ReactiveTrader.Server.Transport
{
    public class ContextHolder : IContextHolder
    {
        public IHubCallerConnectionContext PricingHubClient { get; set; }
        public IHubCallerConnectionContext BlotterHubClients { get; set; }
        public IHubCallerConnectionContext ReferenceDataHubClients { get; set; }
    }
}
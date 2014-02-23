using Adaptive.ReactiveTrader.Server.Transport;
using Microsoft.AspNet.SignalR;
using Owin;

namespace Adaptive.ReactiveTrader.Server
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var hubConfiguration = new HubConfiguration
            {
                // you don't want to use that in prod, just when debugging
                EnableDetailedErrors = true,
                Resolver = new AutofacSignalRDependencyResolver(App.Container)
            };

            app.MapSignalR(hubConfiguration);
        }
    }
}
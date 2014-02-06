using Adaptive.ReactiveTrader.Server.SignalR;
using Autofac;
using Microsoft.AspNet.SignalR;
using Owin;

namespace Adaptive.ReactiveTrader.Server
{
    class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            log4net.Config.XmlConfigurator.Configure();

            var bootstrapper = new Bootstrapper();
            var container = bootstrapper.Build();
            var priceFeed = container.Resolve<IPriceFeed>();
            priceFeed.Start();

            var hubConfiguration = new HubConfiguration
            {
                // you don't want to use that in prod, just when debugging
                EnableDetailedErrors = true,
                Resolver = new AutofacSignalRDependencyResolver(container)
            };
            GlobalHost.HubPipeline.AddModule(new LoggingPipelineModule()); 
            app.MapSignalR(hubConfiguration);
        }
    }
}
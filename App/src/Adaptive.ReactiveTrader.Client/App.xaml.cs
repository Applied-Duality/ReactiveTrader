using System.Windows;
using Adaptive.ReactiveTrader.Client.Transport;
using Autofac;

namespace Adaptive.ReactiveTrader.Client
{
    public partial class App
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            log4net.Config.XmlConfigurator.Configure();

            Start();
        }

        private static void Start()
        {
            var bootstrapper = new Bootstrapper();
            var container = bootstrapper.Build();

            var sampleClient = container.Resolve<ISampleClient>();
            sampleClient.Start();
        }
    }
}

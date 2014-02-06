using System;
using System.Windows;
using System.Windows.Navigation;
using Adaptive.ReactiveTrader.Client.Transport;
using Adaptive.ReactiveTrader.Client.UI.Shell;
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

        private void Start()
        {
            var bootstrapper = new Bootstrapper();
            var container = bootstrapper.Build();

            MainWindow = new MainWindow();
            MainWindow.Show();

            var shellView = container.Resolve<ShellView>();
            MainWindow.Content = shellView;

            // var sampleClient = container.Resolve<ISampleClient>();
            // sampleClient.Start();
        }
    }
}

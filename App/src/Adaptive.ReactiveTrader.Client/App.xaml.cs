using System;
using System.Windows;
using Adaptive.ReactiveTrader.Client.Transport;
using Adaptive.ReactiveTrader.Client.UI.Shell;
using Autofac;
using log4net;

namespace Adaptive.ReactiveTrader.Client
{
    public partial class App
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(App));
        private IDisposable _transportSubscription;

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

            var transport = container.Resolve<ISignalRTransport>();
            _transportSubscription = transport.Initialize("http://localhost:8080", "bob")
                .Subscribe(
                    _ => Log.Info("Transport initialized."),
                    ex => Log.Error("Failed to initialize transport.", ex));
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _transportSubscription.Dispose();
            base.OnExit(e);
        }
    }
}

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
        private IDisposable _connectionProviderSubscription;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            InitializeLogging();

            Start();
        }

        private void Start()
        {
            var bootstrapper = new Bootstrapper();
            var container = bootstrapper.Build();

            var connectionProvider = container.Resolve<IConnectionProvider>();
            _connectionProviderSubscription = connectionProvider.GetActiveConnection()
                .Subscribe(
                connection => Log.InfoFormat("A new connection was created {0}", connection),
                ex => Log.Error("An error occured in connection provider, this should not happen", ex),
                () => Log.Error("Subscription to connection provider completed, this should never happen."));

            MainWindow = new MainWindow();
            MainWindow.Show();

            var shellView = container.Resolve<ShellView>();
            MainWindow.Content = shellView;
        }

        protected override void OnExit(ExitEventArgs e)
        {
            if (_connectionProviderSubscription != null)
            {
                _connectionProviderSubscription.Dispose();                
            }
            base.OnExit(e);
        }

        private void InitializeLogging()
        {
            log4net.Config.XmlConfigurator.Configure();

            Log.Info(@"  _____                 _   _           ");
            Log.Info(@" |  __ \               | | (_)          ");
            Log.Info(@" | |__) |___  __ _  ___| |_ ___   _____ ");
            Log.Info(@" |  _  // _ \/ _` |/ __| __| \ \ / / _ \");
            Log.Info(@" | | \ \  __/ (_| | (__| |_| |\ V /  __/");
            Log.Info(@" |_|  \_\___|\__,_|\___|\__|_| \_/ \___|");
        }
    }
}

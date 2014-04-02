using Adaptive.ReactiveTrader.Client.Concurrency;
using Adaptive.ReactiveTrader.Client.Configuration;
using Adaptive.ReactiveTrader.Client.Domain;
using Adaptive.ReactiveTrader.Client.Domain.Instrumentation;
using Adaptive.ReactiveTrader.Client.UI.Blotter;
using Adaptive.ReactiveTrader.Client.UI.Connectivity;
using Adaptive.ReactiveTrader.Client.UI.Shell;
using Adaptive.ReactiveTrader.Client.UI.SpotTiles;
using Autofac;

namespace Adaptive.ReactiveTrader.Client
{
    public class Bootstrapper
    {
        public IContainer Build()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<Domain.ReactiveTrader>().As<IReactiveTrader>().SingleInstance();
            builder.RegisterType<ConfigurationProvider>().As<IConfigurationProvider>();
            builder.RegisterType<UserProvider>().As<IUserProvider>();
            builder.RegisterType<ConcurrencyService>().As<IConcurrencyService>();

            // views
            builder.RegisterType<ShellView>();
            builder.RegisterType<SpotTilesView>();
            builder.RegisterType<SpotTileView>();
            builder.RegisterType<BlotterView>();
            
            // view models
            builder.RegisterType<ShellViewModel>().As<IShellViewModel>().ExternallyOwned();
            builder.RegisterType<SpotTilesViewModel>().As<ISpotTilesViewModel>().ExternallyOwned();
            builder.RegisterType<SpotTileViewModel>().As<ISpotTileViewModel>().ExternallyOwned();
            builder.RegisterType<SpotTileErrorViewModel>().As<ISpotTileErrorViewModel>().ExternallyOwned();
            builder.RegisterType<SpotTileConfigViewModel>().As<ISpotTileConfigViewModel>().ExternallyOwned();
            builder.RegisterType<SpotTilePricingViewModel>().As<ISpotTilePricingViewModel>().ExternallyOwned();
            builder.RegisterType<OneWayPriceViewModel>().As<IOneWayPriceViewModel>().ExternallyOwned();
            builder.RegisterType<SpotTileAffirmationViewModel>().As<ISpotTileAffirmationViewModel>().ExternallyOwned();
            builder.RegisterType<BlotterViewModel>().As<IBlotterViewModel>().ExternallyOwned();
            builder.RegisterType<TradeViewModel>().As<ITradeViewModel>().ExternallyOwned();
            builder.RegisterType<ConnectivityStatusViewModel>().As<IConnectivityStatusViewModel>().ExternallyOwned();

            return builder.Build();
        } 
    }
}
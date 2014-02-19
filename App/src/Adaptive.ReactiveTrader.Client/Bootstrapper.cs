using Adaptive.ReactiveTrader.Client.Configuration;
using Adaptive.ReactiveTrader.Client.Domain;
using Adaptive.ReactiveTrader.Client.UI.Blotter;
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

            // views
            builder.RegisterType<ShellView>();
            builder.RegisterType<SpotTilesView>();
            builder.RegisterType<SpotTileView>();
            builder.RegisterType<BlotterView>();
            
            // view models
            builder.RegisterType<ShellViewModel>().As<IShellViewModel>();
            builder.RegisterType<SpotTilesViewModel>().As<ISpotTilesViewModel>();
            builder.RegisterType<SpotTileViewModel>().As<ISpotTileViewModel>();
            builder.RegisterType<SpotTilePricingViewModel>().As<ISpotTilePricingViewModel>();
            builder.RegisterType<OneWayPriceViewModel>().As<IOneWayPriceViewModel>();
            builder.RegisterType<SpotTileAffirmationViewModel>().As<ISpotTileAffirmationViewModel>();
            builder.RegisterType<BlotterViewModel>().As<IBlotterViewModel>();
            builder.RegisterType<TradeViewModel>().As<ITradeViewModel>();

            return builder.Build();
        } 
    }
}
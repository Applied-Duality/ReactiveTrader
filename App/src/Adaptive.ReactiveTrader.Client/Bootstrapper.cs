using Adaptive.ReactiveTrader.Client.Configuration;
using Adaptive.ReactiveTrader.Client.Models;
using Adaptive.ReactiveTrader.Client.Repositories;
using Adaptive.ReactiveTrader.Client.ServiceClients.Blotter;
using Adaptive.ReactiveTrader.Client.ServiceClients.Execution;
using Adaptive.ReactiveTrader.Client.ServiceClients.Pricing;
using Adaptive.ReactiveTrader.Client.ServiceClients.ReferenceData;
using Adaptive.ReactiveTrader.Client.Transport;
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

            builder.RegisterType<ConnectionProvider>().As<IConnectionProvider>().SingleInstance();
            builder.RegisterType<ConfigurationProvider>().As<IConfigurationProvider>();
            builder.RegisterType<UserProvider>().As<IUserProvider>();

            // service clients
            builder.RegisterType<ReferenceDataServiceClient>().As<IReferenceDataServiceClient>().SingleInstance();
            builder.RegisterType<ExecutionServiceClient>().As<IExecutionServiceClient>().SingleInstance();
            builder.RegisterType<BlotterServiceClient>().As<IBlotterServiceClient>().SingleInstance();
            builder.RegisterType<PricingServiceClient>().As<IPricingServiceClient>().SingleInstance();

            // repositories
            builder.RegisterType<TradeRepository>().As<ITradeRepository>().SingleInstance();
            builder.RegisterType<ExecutionRepository>().As<IExecutionRepository>().SingleInstance();
            builder.RegisterType<PriceRepository>().As<IPriceRepository>().SingleInstance();
            builder.RegisterType<ReferenceDataRepository>().As<IReferenceDataRepository>().SingleInstance();

            // model factories
            builder.RegisterType<CurrencyPairFactory>().As<ICurrencyPairFactory>().SingleInstance();
            builder.RegisterType<TradeFactory>().As<ITradeFactory>().SingleInstance();
            builder.RegisterType<PriceFactory>().As<IPriceFactory>().SingleInstance();

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
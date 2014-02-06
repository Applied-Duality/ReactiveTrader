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

            builder.RegisterType<SignalRTransport>().As<ISignalRTransport>().As<ITransport>().SingleInstance();

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

            builder.RegisterType<ShellView>();
            builder.RegisterType<ShellViewModel>().As<IShellViewModel>();
            builder.RegisterType<SpotTilesView>();
            builder.RegisterType<SpotTilesViewModel>().As<ISpotTilesViewModel>();
            builder.RegisterType<SpotTileView>();
            builder.RegisterType<SpotTileViewModel>().As<ISpotTileViewModel>();
            builder.RegisterType<BlotterView>();
            builder.RegisterType<BlotterViewModel>().As<IBlotterViewModel>();

            return builder.Build();
        } 
    }
}
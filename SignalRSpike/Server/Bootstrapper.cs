using Autofac;
using Server.SignalR;

namespace Server
{
    public class Bootstrapper
    {
        public IContainer Build()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<PricePublisher>().As<IPricePublisher>().SingleInstance();
            builder.RegisterType<CurrencyPairRepository>().As<ICurrencyPairRepository>().SingleInstance();
            builder.RegisterType<PriceFeedSimulator>().As<IPriceFeed>().SingleInstance();
            builder.RegisterType<PriceLastValueCache>().As<IPriceLastValueCache>().SingleInstance();
            builder.RegisterType<ContextHolder>().As<IContextHolder>().SingleInstance();
            builder.RegisterType<ExecutionService>().As<IExecutionService>().SingleInstance();
            builder.RegisterType<BlotterPublisher>().As<IBlotterPublisher>().SingleInstance();
            builder.RegisterType<TradeRepository>().As<ITradeRepository>().SingleInstance();
            builder.RegisterType<TradingServiceHub>().SingleInstance();

            return builder.Build();
        }
    }
}
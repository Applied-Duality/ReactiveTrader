using Autofac;

namespace Client
{
    public class Bootstrapper
    {
        public IContainer Build()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<Transport>().As<ITransport>().SingleInstance();
            builder.RegisterType<SpotStreamRepository>().As<ISpotStreamRepository>().SingleInstance();
            builder.RegisterType<ExecutionServiceClient>().As<IExecutionServiceClient>().SingleInstance();
            builder.RegisterType<TradeRepository>().As<ITradeRepository>().SingleInstance();
            builder.RegisterType<CurrencyPairRepository>().As<ICurrencyPairRepository>().SingleInstance();
            builder.RegisterType<SampleClient>().As<ISampleClient>().SingleInstance();

            return builder.Build();
        } 
    }
}
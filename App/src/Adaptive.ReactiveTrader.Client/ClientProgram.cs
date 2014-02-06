using System;
using Autofac;

namespace Adaptive.ReactiveTrader.Client
{
    class ClientProgram
    {
        static void Main()
        {
            log4net.Config.XmlConfigurator.Configure();

            Start();

            Console.ReadKey();
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

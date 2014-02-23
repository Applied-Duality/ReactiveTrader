using System;
using System.Reactive.Linq;
using Adaptive.ReactiveTrader.Client.Domain.Models;
using Adaptive.ReactiveTrader.Client.Domain.Repositories;
using Adaptive.ReactiveTrader.Client.Domain.ServiceClients;
using Adaptive.ReactiveTrader.Client.Domain.Transport;
using log4net;

namespace Adaptive.ReactiveTrader.Client.Domain
{
    public class ReactiveTrader : IReactiveTrader
    {
        private ConnectionProvider _connectionProvider;
        private static readonly ILog Log = LogManager.GetLogger(typeof(ReactiveTrader));

        public void Initialize(string username, string[] servers)
        {
            _connectionProvider = new ConnectionProvider(username, servers);

            var referenceDataServiceClient = new ReferenceDataServiceClient(_connectionProvider);
            var executionServiceClient = new ExecutionServiceClient(_connectionProvider);
            var blotterServiceClient = new BlotterServiceClient(_connectionProvider);
            var pricingServiceClient = new PricingServiceClient(_connectionProvider);

            var tradeFactory = new TradeFactory();
            var executionRepository = new ExecutionRepository(executionServiceClient, tradeFactory);
            var priceFactory = new PriceFactory(executionRepository);
            var priceRepository = new PriceRepository(pricingServiceClient, priceFactory);
            var currencyPairFactory = new CurrencyPairFactory(priceRepository);
            TradeRepository = new TradeRepository(blotterServiceClient, tradeFactory);
            ReferenceData = new ReferenceDataRepository(referenceDataServiceClient, currencyPairFactory);
        }

        public IReferenceDataRepository ReferenceData { get; private set; }
        public ITradeRepository TradeRepository { get; private set; }

        public IObservable<ConnectionInfo> ConnectionStatus
        {
            get
            {
                return _connectionProvider.GetActiveConnection()
                    .Do(_ => Log.Info("New connection created by connection provider"))
                    .Select(c => c.Status)
                    .Switch()
                    .Publish()
                    .RefCount();
            }
        }
    }
}

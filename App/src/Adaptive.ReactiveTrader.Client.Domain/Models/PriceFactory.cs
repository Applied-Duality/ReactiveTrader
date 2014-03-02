using Adaptive.ReactiveTrader.Client.Domain.Repositories;
using Adaptive.ReactiveTrader.Shared.Pricing;

namespace Adaptive.ReactiveTrader.Client.Domain.Models
{
    class PriceFactory : IPriceFactory
    {
        private readonly IExecutionRepository _executionRepository;

        public PriceFactory(IExecutionRepository executionRepository)
        {
            _executionRepository = executionRepository;
        }

        public IPrice Create(PriceDto priceDto, ICurrencyPair currencyPair)
        {
            var bid = new ExecutablePrice(Direction.Sell, priceDto.Bid, _executionRepository);
            var ask = new ExecutablePrice(Direction.Buy, priceDto.Ask, _executionRepository);
            var price = new Price(bid, ask, priceDto.Mid, priceDto.QuoteId, priceDto.ValueDate, currencyPair);

            return price;
        }
    }
}
using Adaptive.ReactiveTrader.Client.Domain.Models.ReferenceData;
using Adaptive.ReactiveTrader.Client.Domain.Repositories;
using Adaptive.ReactiveTrader.Shared.DTO.Pricing;

namespace Adaptive.ReactiveTrader.Client.Domain.Models.Pricing
{
    internal class PriceFactory : IPriceFactory
    {
        private readonly IExecutionRepository _executionRepository;

        public PriceFactory(IExecutionRepository executionRepository)
        {
            _executionRepository = executionRepository;
        }

        public IPrice Create(PriceDto priceDto, ICurrencyPair currencyPair)
        {
            var bid = new ExecutablePrice(Direction.SELL, priceDto.Bid, _executionRepository);
            var ask = new ExecutablePrice(Direction.BUY, priceDto.Ask, _executionRepository);
            var price = new Price(bid, ask, priceDto.Mid, priceDto.QuoteId, priceDto.ValueDate, currencyPair);

            return price;
        }
    }
}
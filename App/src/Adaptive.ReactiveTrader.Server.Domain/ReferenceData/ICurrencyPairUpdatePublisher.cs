using System.Threading.Tasks;
using Adaptive.ReactiveTrader.Shared.DTO.ReferenceData;

namespace Adaptive.ReactiveTrader.Server.ReferenceData
{
    public interface ICurrencyPairUpdatePublisher
    {
        Task AddCurrencyPair(CurrencyPairDto ccyPair);
        Task RemoveCurrencyPair(CurrencyPairDto ccyPair);
    }
}
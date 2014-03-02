using System.Threading.Tasks;
using Adaptive.ReactiveTrader.Shared.ReferenceData;

namespace Adaptive.ReactiveTrader.Server.ReferenceData
{
    public interface ICurrencyPairUpdatePublisher
    {
        Task Publish(CurrencyPairUpdateDto update);
    }
}
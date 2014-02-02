using System.Threading.Tasks;
using Dto.Pricing;

namespace Server
{
    internal interface IPricePublisher
    {
        Task Publish(SpotPrice price);
    }
}
using System.Threading.Tasks;
using Dto;

namespace Server
{
    internal interface IBlotterPublisher
    {
        Task Publish(SpotTrade trade);
    }
}
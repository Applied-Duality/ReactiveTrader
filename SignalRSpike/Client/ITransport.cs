using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client;

namespace Client
{
    public interface ITransport
    {
        Task Initialize(string address, string userName);
        IHubProxy HubProxy { get; }
    }
}
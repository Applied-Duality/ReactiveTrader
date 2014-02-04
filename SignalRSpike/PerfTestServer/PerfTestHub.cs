using System;
using Microsoft.AspNet.SignalR;

namespace PerfTestServer
{
    public class PerfTestHub : Hub
    {
        public void RegisterClient()
        {
            PriceFeed.Instance.Context = Clients;

            Groups.Add(Context.ConnectionId, "perfSubject");

            Console.WriteLine("Client registered and added to  perfSubject group");
        }
    }
}
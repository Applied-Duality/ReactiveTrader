using System;
using System.Threading;
using Dto.Pricing;
using Microsoft.AspNet.SignalR.Client;

namespace PerfTestClient
{
    public class PerfTestAgent
    {
        private readonly int _agentId;
        private volatile int _priceCount;
        private Timer _timer;

        public PerfTestAgent(int agentId)
        {
            _agentId = agentId;
        }

        public async void Start()
        {
            var hubConnection = new HubConnection("http://localhost:8080");

            var hub = hubConnection.CreateHubProxy("PerfTestHub");
            hub.On<SpotPrice>("OnNewPrice", OnNewPrice);

            await hubConnection.Start();
            await hub.Invoke("RegisterClient");

            _timer = new Timer(OnTimerTick, null, 0, 10000);
        }

        private void OnTimerTick(object state)
        {
            Console.WriteLine("Agent[{0}]: Received {1} updates per second", _agentId, _priceCount / 10);
            _priceCount = 0;
        }

        private void OnNewPrice(SpotPrice obj)
        {
            _priceCount++;
        }
    }
}
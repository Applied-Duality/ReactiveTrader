using System;
using System.Diagnostics;
using System.Threading;
using Microsoft.AspNet.SignalR.Client;
using PerfTestDto;

namespace PerfTestClient
{
    public class PerfTestAgent
    {
        private readonly int _agentId;
        private volatile int _priceCount;
        private Timer _timer;
        private long _cumulativeLatencyTicks;

        public PerfTestAgent(int agentId)
        {
            _agentId = agentId;
        }

        public async void Start()
        {
            var hubConnection = new HubConnection("http://localhost:8080");

            var hub = hubConnection.CreateHubProxy("PerfTestHub");
            hub.On<PerfTestSpotPrice>("OnNewPrice", OnNewPrice);

            await hubConnection.Start();
            await hub.Invoke("RegisterClient");

            _timer = new Timer(OnTimerTick, null, 0, 10000);
        }

        private void OnTimerTick(object state)
        {
            var latencyMs = ((double) _cumulativeLatencyTicks/Stopwatch.Frequency)*1000.0/_priceCount;

            Console.WriteLine("Agent[{0}]: Received {1} updates per second, average latency {2:0.0}ms", _agentId, _priceCount / 10, latencyMs);
            _priceCount = 0;
            _cumulativeLatencyTicks = 0;
        }

        private void OnNewPrice(PerfTestSpotPrice priceUpdate)
        {
            _priceCount++;
            _cumulativeLatencyTicks += Stopwatch.GetTimestamp() - priceUpdate.Timestamp;
        }
    }
}
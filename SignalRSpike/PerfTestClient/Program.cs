using System;
using System.Threading;
using Dto.Pricing;
using Microsoft.AspNet.SignalR.Client;

namespace PerfTestClient
{
    class Program
    {
        private static volatile int _priceCount;
        private static Timer _timer;

        static void Main(string[] args)
        {
            Start();

            Console.ReadKey();
        }

        private static async void Start()
        {
            var hubConnection = new HubConnection("http://localhost:8080");

            var hub = hubConnection.CreateHubProxy("PerfTestHub");
            hub.On<SpotPrice>("OnNewPrice", OnNewPrice);

            await hubConnection.Start();
            Console.WriteLine("connection started");

            await hub.Invoke("RegisterClient");

            Console.WriteLine("Client registered");

            _timer = new Timer(OnTimerTick, null, 0, 1000);
        }

        private static void OnTimerTick(object state)
        {
            Console.WriteLine("Received {0} updates per second", _priceCount);
            _priceCount = 0;
        }

        private static void OnNewPrice(SpotPrice obj)
        {
            _priceCount++;
        }
    }
}

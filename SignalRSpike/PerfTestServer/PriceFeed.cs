using System;
using System.Threading;
using Dto.Pricing;
using Microsoft.AspNet.SignalR.Hubs;

namespace PerfTestServer
{
    internal class PriceFeed
    {
        public static readonly PriceFeed Instance = new PriceFeed();
        private Timer _timer;
        private SpotPrice _quote;
        private const int UpdatesPerTick = 2;
        private const int UpdatePeriodMs = 5;

        private PriceFeed()
        {
        }

        public void Start()
        {
            _quote = new SpotPrice
            {
                QuoteId = 0,
                Ask = 1.2345m,
                Bid = 1.2346m,
                Mid = 1.23455m,
                Symbol = "EURUSD",
                ValueDate = DateTime.Now
            };
            _timer = new Timer(OnTimerTick, null, UpdatePeriodMs, UpdatePeriodMs);
        }

        public IHubCallerConnectionContext Context { get; set; }

        private void OnTimerTick(object state)
        {
            if (Context == null) return;

            for (int i = 0; i < UpdatesPerTick; i++)
            {
                Context.Group("perfSubject").OnNewPrice(_quote);
            }
        }
    }
}
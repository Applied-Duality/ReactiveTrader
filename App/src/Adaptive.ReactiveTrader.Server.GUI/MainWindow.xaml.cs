using System;
using System.Threading;
using Adaptive.ReactiveTrader.Server.Pricing;
using log4net;
using Microsoft.Owin.Hosting;

namespace Adaptive.ReactiveTrader.Server
{
    public partial class MainWindow
    {
        private const string Address = "http://localhost:8080";
        private static readonly ILog Log = LogManager.GetLogger(typeof(MainWindow));

        private readonly IPricePublisher _pricePublisher;
        private readonly IPriceFeed _priceFeed;

        private IDisposable _signalr;
        private Timer _timer;
        private long _lastTickTotalUpdates;

        public MainWindow(IPricePublisher pricePublisher, IPriceFeed priceFeed)
        {
            _pricePublisher = pricePublisher;
            _priceFeed = priceFeed;
            InitializeComponent();

            StartServer();
        }

        private void ServerButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (_signalr != null)
            {
                StopServer();
            }
            else
            {
                StartServer();
            }
        }

        private void StopServer()
        {
            _signalr.Dispose();
            _signalr = null;
            ServerStatusTextBlock.Text = "Stoped";
            ServerButton.Content = "Start";

            if (_timer != null)
            {
                _timer.Dispose();
                _timer = null;
                ThroughputTextBlock.Text = "0";
            }
        }

        private void StartServer()
        {
            ServerStatusTextBlock.Text = "Starting...";
            try
            {
                _signalr = WebApp.Start(Address);
            }
            catch (Exception exception)
            {
                Log.Error("An error occured while starting SignalR", exception);
            }
            ServerStatusTextBlock.Text = "Started on " + Address;
            ServerButton.Content = "Stop";
            StartMeasuringThroughput();
        }

        private void StartMeasuringThroughput()
        {
            _lastTickTotalUpdates = _pricePublisher.TotalPricesPublished;
            _timer = new Timer(state =>
            {
                var newTotalUpdates = _pricePublisher.TotalPricesPublished;
                var publishedLastSecond = newTotalUpdates - _lastTickTotalUpdates;
                _lastTickTotalUpdates = newTotalUpdates;

                Dispatcher.BeginInvoke((Action)(() =>
                {
                    ThroughputTextBlock.Text = publishedLastSecond.ToString("N0");
                }));
                
            }, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
        }

        private void Slider_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {
            _priceFeed.SetUpdateFrequency(e.NewValue);
        }
    }
}

using System;
using log4net;
using Microsoft.Owin.Hosting;

namespace Adaptive.ReactiveTrader.Server
{
    public partial class MainWindow
    {
        private IDisposable _signalr;
        private const string Address = "http://localhost:8080";
        private static readonly ILog Log = LogManager.GetLogger(typeof(MainWindow));

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ServerButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            if (_signalr != null)
            {
                _signalr.Dispose();
                _signalr = null;
                ServerStatusTextBlock.Text = "Stoped";
                ServerButton.Content = "Start";
            }
            else
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
            }
        }

        private void Slider_ValueChanged(object sender, System.Windows.RoutedPropertyChangedEventArgs<double> e)
        {

        }
    }
}

using System.Diagnostics;
using System.Windows.Navigation;

namespace Adaptive.ReactiveTrader.Client.UI.Connectivity
{
    public partial class ConnectivityStatusView
    {
        public ConnectivityStatusView()
        {
            InitializeComponent();
        }

        private void Hyperlink_OnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start("http://www.weareadaptive.com");
        }
    }
}

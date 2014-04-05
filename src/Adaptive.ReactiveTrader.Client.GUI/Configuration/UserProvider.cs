using System.Diagnostics;

namespace Adaptive.ReactiveTrader.Client.Configuration
{
    class UserProvider : IUserProvider
    {
        public string Username
        {
            get
            {
                return "Trader-" + Process.GetCurrentProcess().Id;
            }
        }
    }
}
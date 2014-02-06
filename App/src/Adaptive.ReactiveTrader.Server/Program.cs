using System;
using Adaptive.ReactiveTrader.Server.Transport;
using log4net;
using Microsoft.Owin;
using Microsoft.Owin.Hosting;

[assembly: OwinStartup(typeof(Startup))]

namespace Adaptive.ReactiveTrader.Server
{
    class Program
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(Program));

        static void Main()
        {
            // This will *ONLY* bind to localhost, if you want to bind to all addresses
            // use http://*:8080 to bind to all addresses. 
            // See http://msdn.microsoft.com/en-us/library/system.net.httplistener.aspx 
            // for more information.
            const string url = "http://localhost:8080";
            using (WebApp.Start(url))
            {
                Log.InfoFormat("Server running on {0}", url);
                Console.ReadLine();
            }
        }
    }
}
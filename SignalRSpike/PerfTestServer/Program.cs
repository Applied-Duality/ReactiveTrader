using System;
using Microsoft.Owin;
using Microsoft.Owin.Hosting;
using PerfTestServer;

[assembly: OwinStartup(typeof(Startup))]

namespace PerfTestServer
{
    class Program
    {
        static void Main(string[] args)
        {
            PriceFeed.Instance.Start();

            // This will *ONLY* bind to localhost, if you want to bind to all addresses
            // use http://*:8080 to bind to all addresses. 
            // See http://msdn.microsoft.com/en-us/library/system.net.httplistener.aspx 
            // for more information.
            const string url = "http://localhost:8080";
            using (WebApp.Start(url))
            {
                Console.WriteLine("Server running on {0}", url);
                Console.ReadLine();
            }
        }
    }
}

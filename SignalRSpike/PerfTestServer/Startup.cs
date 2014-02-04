using Owin;

namespace PerfTestServer
{
    class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}
using log4net;
using Microsoft.AspNet.SignalR.Hubs;

namespace Adaptive.ReactiveTrader.Server.Transport
{
    public class LoggingPipelineModule : HubPipelineModule
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(LoggingPipelineModule));

        protected override bool OnBeforeIncoming(IHubIncomingInvokerContext context)
        {
            if (Log.IsDebugEnabled)
            {
                Log.DebugFormat("=> Invoking " + context.MethodDescriptor.Name + " on hub " + context.MethodDescriptor.Hub.Name);
            }
            return base.OnBeforeIncoming(context);
        }
        protected override bool OnBeforeOutgoing(IHubOutgoingInvokerContext context)
        {
            if (Log.IsDebugEnabled)
            {
                Log.DebugFormat("<= Invoking {0} on client hub {1}, data: {2}", context.Invocation.Method, context.Invocation.Hub, string.Join(", ", context.Invocation.Args));
            }
            return base.OnBeforeOutgoing(context);
        }
    }
}
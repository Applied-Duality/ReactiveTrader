using System;
using System.Runtime.Serialization;

namespace Adaptive.ReactiveTrader.Client.Domain.ServiceClients
{
    [Serializable]
    public class TransportDisconnectedException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public TransportDisconnectedException()
        {
        }

        public TransportDisconnectedException(string message) : base(message)
        {
        }

        public TransportDisconnectedException(string message, Exception inner) : base(message, inner)
        {
        }

        protected TransportDisconnectedException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
using System;
using Autofac;
using Microsoft.AspNet.SignalR;

namespace Adaptive.ReactiveTrader.Server.Transport
{
    public class AutofacSignalRDependencyResolver : DefaultDependencyResolver
    {
        private readonly IContainer _container;

        public AutofacSignalRDependencyResolver(IContainer container)
        {
            _container = container;
        }

        public override object GetService(Type serviceType)
        {
            object instance;
            if (_container.TryResolve(serviceType, out instance))
            {
                return instance;
            }
            return base.GetService(serviceType);
        }
    }
}
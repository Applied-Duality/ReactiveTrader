using System;
using System.Collections.Generic;
using Adaptive.ReactiveTrader.Contracts.Execution;

namespace Adaptive.ReactiveTrader.Client.ServiceClients.Blotter
{
    public interface IBlotterServiceClient
    {
        IObservable<IEnumerable<Trade>> GetTrades();
    }
}
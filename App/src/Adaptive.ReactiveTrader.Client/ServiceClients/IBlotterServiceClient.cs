using System;
using System.Collections.Generic;
using Adaptive.ReactiveTrader.Shared.Execution;

namespace Adaptive.ReactiveTrader.Client.ServiceClients
{
    public interface IBlotterServiceClient
    {
        IObservable<IEnumerable<TradeDto>> GetTrades();
    }
}
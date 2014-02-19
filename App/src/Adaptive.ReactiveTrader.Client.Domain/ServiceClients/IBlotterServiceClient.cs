using System;
using System.Collections.Generic;
using Adaptive.ReactiveTrader.Shared.Execution;

namespace Adaptive.ReactiveTrader.Client.Domain.ServiceClients
{
    interface IBlotterServiceClient
    {
        IObservable<IEnumerable<TradeDto>> GetTrades();
    }
}
using System;
using Adaptive.ReactiveTrader.Client.Domain.Repositories;

namespace Adaptive.ReactiveTrader.Client.Domain
{
    public interface IReactiveTrader
    {
        IReferenceDataRepository ReferenceData { get; }
        ITradeRepository TradeRepository { get; }
        IObservable<ConnectionInfo> ConnectionStatusStream { get; }
        void Initialize(string username, string[] servers);
    }
}
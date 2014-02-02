using System;
using Dto;

namespace Client
{
    public interface ITradeRepository
    {
        IObservable<SpotTrade> GetAllTrades();
    }
}
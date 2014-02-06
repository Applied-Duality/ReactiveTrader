using System;
using Adaptive.ReactiveTrader.Client.Models;

namespace Adaptive.ReactiveTrader.Client.Repositories
{
    public interface IPriceRepository
    {
        IObservable<IPrice> GetPrices(ICurrencyPair currencyPair);
    }
}
using System;
using Adaptive.ReactiveTrader.Client.Domain.Models;

namespace Adaptive.ReactiveTrader.Client.Domain.Repositories
{
    interface IPriceRepository
    {
        IObservable<IPrice> GetPrices(ICurrencyPair currencyPair);
    }
}
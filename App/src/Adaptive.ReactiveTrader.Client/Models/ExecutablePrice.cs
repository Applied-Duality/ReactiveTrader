using System;
using Adaptive.ReactiveTrader.Client.Repositories;
using Adaptive.ReactiveTrader.Shared.Extensions;

namespace Adaptive.ReactiveTrader.Client.Models
{
    class ExecutablePrice : IExecutablePrice
    {
        private readonly IExecutionRepository _executionRepository;

        public ExecutablePrice(Direction direction, decimal rate, IExecutionRepository executionRepository)
        {
            _executionRepository = executionRepository;
            Direction = direction;
            Rate = rate;
        }

        public IObservable<ITrade> Execute(long notional)
        {
            return _executionRepository.Execute(this, notional)
                .CacheFirstResult();
        }

        public Direction Direction { get; private set; }
        public decimal Rate { get; private set; }
        public IPrice Parent { get; internal set; }
    }
}
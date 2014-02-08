using System;
using Adaptive.ReactiveTrader.Client.Repositories;
using Adaptive.ReactiveTrader.Shared.Extensions;

namespace Adaptive.ReactiveTrader.Client.Models
{
    class ExecutablePrice : IExecutablePrice
    {
        private readonly IExecutionRepository _executionRepository;

        public ExecutablePrice(string bigNumbers, string pips, string pipDecimals, Direction direction, decimal rate, IExecutionRepository executionRepository)
        {
            _executionRepository = executionRepository;
            BigNumbers = bigNumbers;
            Pips = pips;
            PipDecimals = pipDecimals;
            Direction = direction;
            Rate = rate;
        }

        public IObservable<ITrade> Execute(long notional)
        {
            return _executionRepository.Execute(this, notional)
                .CacheFirstResult();
        }

        public string BigNumbers { get; private set; }
        public string Pips { get; private set; }
        public string PipDecimals { get; private set; }
        public Direction Direction { get; private set; }
        public decimal Rate { get; private set; }
        public IPrice Parent { get; internal set; }
    }
}
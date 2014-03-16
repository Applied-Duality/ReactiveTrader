class ExecutablePrice implements IExecutablePrice {
    private _executionRepository: IExecutionRepository;

    constructor(direction: Direction, rate: number, executionRepository: IExecutionRepository) {
        this._executionRepository = executionRepository;
        this.direction = direction;
        this.rate = rate;
    }

    execute(notional: number, dealtCurrency: string) {
        return this._executionRepository.execute(this, notional, dealtCurrency)
            .cacheFirstResult();
    }

    // TODO encapsulate
    direction: Direction;
    rate: number;
    parent: IPrice;
}


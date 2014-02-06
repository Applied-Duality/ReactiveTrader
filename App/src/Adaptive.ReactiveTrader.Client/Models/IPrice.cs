namespace Adaptive.ReactiveTrader.Client.Models
{
    public interface IPrice
    {
        IExecutablePrice Bid { get; }
        IExecutablePrice Ask { get; }
    }
}

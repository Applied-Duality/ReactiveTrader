namespace Adaptive.ReactiveTrader.Client.UI.Connectivity
{
    public interface IConnectivityStatusViewModel
    {
        string Status { get; }
        long UiLatency { get; }
        long Throughput { get; }
    }
}
namespace Adaptive.ReactiveTrader.Client.UI.Connectivity
{
    public interface IConnectivityStatusViewModel
    {
        string Status { get; }
        long Throughput { get; }
        bool Disconnected { get; }
        long ServerClientLatency { get; }
        long UiLatency { get; }
        long ServerClientLatencyStdDev { get; }
        long UiLatencyStdDev { get; }
        double CpuTime { get; }
        double CpuPercent { get; }
    }
}
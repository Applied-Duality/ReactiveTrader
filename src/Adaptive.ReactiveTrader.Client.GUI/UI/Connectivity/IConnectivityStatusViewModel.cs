namespace Adaptive.ReactiveTrader.Client.UI.Connectivity
{
    public interface IConnectivityStatusViewModel
    {
        string Status { get; }
        bool Disconnected { get; }
        
        long UiUpdates { get; }
        long TicksReceived { get; }
        long ServerClientLatency { get; }
        long UiLatency { get; }
        string Histogram { get; }
        
        double CpuTime { get; }
        double CpuPercent { get; }
    }
}
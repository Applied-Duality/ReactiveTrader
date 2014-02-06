namespace Adaptive.ReactiveTrader.Client.Transport
{
    public enum TransportStatus
    {
        Connecting,
        Connected,
        ConnectionSlow,
        Reconnecting,
        Reconnected,
        Closed,
        Uninitialized
    }
}
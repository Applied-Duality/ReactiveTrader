namespace Adaptive.ReactiveTrader.Client.Domain.Models.Pricing
{
    public interface IPriceLatency
    {
        double ServerToClientMs { get; }
        double UiProcessingTimeMs { get; }
        void DisplayedOnUi();
        double TotalLatencyMs { get; }
    }
}
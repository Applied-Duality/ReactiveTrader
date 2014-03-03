using System.Windows;

namespace Adaptive.ReactiveTrader.Client.UI.Behaviors
{
    public class MaximizeWindowButtonBehavior : WindowButtonBehavior
    {
        protected override void OnButtonClicked()
        {
            AssociatedWindow.WindowState = AssociatedWindow.WindowState == WindowState.Normal ? WindowState.Maximized : WindowState.Normal;
        }
    }
}

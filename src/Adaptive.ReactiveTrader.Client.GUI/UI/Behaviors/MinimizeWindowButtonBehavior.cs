using System.Windows;

namespace Adaptive.ReactiveTrader.Client.UI.Behaviors
{
    public class MinimizeWindowButtonBehavior : WindowButtonBehavior
    {
        protected override void OnButtonClicked()
        {
            AssociatedWindow.WindowState = WindowState.Minimized;
        }
    }
}

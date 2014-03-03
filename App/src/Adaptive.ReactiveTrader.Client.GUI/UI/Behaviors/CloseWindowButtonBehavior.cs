namespace Adaptive.ReactiveTrader.Client.UI.Behaviors
{
    public class CloseWindowButtonBehavior : WindowButtonBehavior
    {
        protected override void OnButtonClicked()
        {
            AssociatedWindow.Close();
        }
    }
}

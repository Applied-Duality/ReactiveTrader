using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace Adaptive.ReactiveTrader.Client.UI.Behaviors
{
    public abstract class WindowButtonBehavior : Behavior<Button>
    {
        public static readonly DependencyProperty AssociatedWindowProperty = DependencyProperty.Register(
            "AssociatedWindow", typeof(Window), typeof(WindowButtonBehavior), new PropertyMetadata(null));

        public Window AssociatedWindow
        {
            get { return (Window) GetValue(AssociatedWindowProperty); }
            set { SetValue(AssociatedWindowProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.Click += AssociatedObjectClick;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.Click -= AssociatedObjectClick;
        }

        private void AssociatedObjectClick(object sender, RoutedEventArgs e)
        {
            if (AssociatedWindow != null)
            {
                OnButtonClicked();
            }
        }

        protected abstract void OnButtonClicked();
    }
}

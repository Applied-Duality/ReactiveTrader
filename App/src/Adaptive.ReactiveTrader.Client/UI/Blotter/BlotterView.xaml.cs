namespace Adaptive.ReactiveTrader.Client.UI.Blotter
{
    public partial class BlotterView
    {
        public BlotterView()
        {
            InitializeComponent();
        }

        public BlotterView(IBlotterViewModel viewModel)
            : this()
        {
            DataContext = viewModel;
        }
    }
}

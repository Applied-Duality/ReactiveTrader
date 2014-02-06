namespace Adaptive.ReactiveTrader.Client.UI.SpotTiles
{
    public partial class SpotTilesView
    {
        public SpotTilesView()
        {
            InitializeComponent();
        }

        public SpotTilesView(ISpotTilesViewModel viewModel)
            : this()
        {
            DataContext = viewModel;
        }
    }
}

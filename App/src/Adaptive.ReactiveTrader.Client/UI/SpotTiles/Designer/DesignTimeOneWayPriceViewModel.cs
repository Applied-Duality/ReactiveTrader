using System;
using System.Windows.Input;
using Adaptive.ReactiveTrader.Client.Domain.Models;
using Adaptive.ReactiveTrader.Shared.UI;

namespace Adaptive.ReactiveTrader.Client.UI.SpotTiles.Designer
{
    public class DesignTimeOneWayPriceViewModel : ViewModelBase, IOneWayPriceViewModel
    {
        public DesignTimeOneWayPriceViewModel(Direction direction, string bigFigures, string pips, string tenthOfPip, PriceMovement movement)
        {
            Direction = direction;
            BigFigures = bigFigures;
            Pips = pips;
            TenthOfPip = tenthOfPip;
            Movement = movement;
        }

        public Direction Direction { get; private set; }
        public string BigFigures { get; private set; }
        public string Pips { get; private set; }
        public string TenthOfPip { get; private set; }
        public PriceMovement Movement { get; private set; }
        public ICommand ExecuteCommand { get; private set; }
        public bool IsExecuting { get; private set; }

        public void OnPrice(IExecutablePrice executablePrice)
        {
            throw new NotImplementedException();
        }

        public void OnStalePrice()
        {
            throw new NotImplementedException();
        }
    }
}
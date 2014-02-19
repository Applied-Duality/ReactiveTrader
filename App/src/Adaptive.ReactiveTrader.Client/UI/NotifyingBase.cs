using System.ComponentModel;
using System.Runtime.CompilerServices;
using Adaptive.ReactiveTrader.Client.Properties;

namespace Adaptive.ReactiveTrader.Client.UI
{
    public class NotifyingBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            } 
        }
    }
}

using System.ComponentModel;
using System.Windows.Controls;

namespace Ost.WPFUtil.MVVM
{
    public class View : UserControl, IPropertyOwner, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void NotifyPropChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public void PropertySet<T>(ref T property, T value, string propertyName)
        {
            property = value;
            NotifyPropChanged(propertyName);
        }
    }
}

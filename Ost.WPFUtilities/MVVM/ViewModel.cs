using System.ComponentModel;

namespace Ost.WPFUtil.MVVM
{
    public class ViewModel : IPropertyOwner, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public void PropertySet<T>(ref T p, T v, string propName)
        {
            p = v;
            NotifyPropChanged(propName);
        }
        protected void NotifyPropChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}

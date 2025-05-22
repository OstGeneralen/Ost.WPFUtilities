using System.ComponentModel;

namespace Ost.WpfUtils.MVVM
{
    public class ViewModel : IPropertyOwner, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        public void PropertySet<T>(ref T p, T v, string propName)
        {
            p = v;
            NotifyPropChanged(propName);
        }
        public void PropertySet<T>(ref T property, T value, string propertyName, Action invoke)
        {
            PropertySet(ref property, value, propertyName);
            invoke();
        }
        public void PropertySet<T>(ref T property, T value, string propertyName, Action<T> invoke)
        {
            PropertySet(ref property, value, propertyName);
            invoke(value);
        }
        public void PropertySet<T>(ref T property, T value, string propertyName, Action<T, T> invoke)
        {
            var oldVal = property;
            PropertySet(ref property, value, propertyName);
            invoke(oldVal, property);
        }
        protected void NotifyPropChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}

using System.ComponentModel;
using System.Windows.Controls;

namespace Ost.WpfUtils.MVVM
{
    public class View : UserControl, IPropertyOwner, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        protected static void StaticInitializeView<T>() where T: View
        {
            DepProps.RegisterDependencyProperties(typeof(T));
        }

        protected void NotifyPropChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void PropertySet<T>(ref T property, T value, string propertyName)
        {
            property = value;
            NotifyPropChanged(propertyName);
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

    }

}

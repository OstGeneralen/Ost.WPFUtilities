namespace Ost.WpfUtils.MVVM
{
    /// <summary>
    /// Interface for a type that owns properties that should be part of the WPF MVVM data model
    /// </summary>
    public interface IPropertyOwner
    {
        /// <summary>
        /// Utility one-line for property setters (set => PropertySet(ref property, value, nameof(Property))
        /// </summary>
        /// <typeparam name="T">Type of the property (usually able to deduce from context)</typeparam>
        /// <param name="property">Reference to the property</param>
        /// <param name="value">Value to set</param>
        /// <param name="propertyName">Name of the property (nameof(Property))</param>
        void PropertySet<T>(ref T property, T value, string propertyName);
    }
}

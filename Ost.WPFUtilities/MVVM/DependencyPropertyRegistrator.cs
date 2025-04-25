using System.Reflection;
using System.Windows;

namespace Ost.WPFUtil.MVVM
{
    public class DependencyPropertyRegistrator<T> where T: DependencyObject
    {
        /// <summary>
        /// Registers a normal Dependency property with no OnChange callback
        /// </summary>
        /// <typeparam name="TProp">Type of the property</typeparam>
        /// <param name="propertyName">Name of the property</param>
        /// <param name="defaultValue">Default value of the property</param>
        /// <returns>The registered Dependency Property</returns>
        public DependencyProperty Register<TProp>( string propertyName, TProp defaultValue)
        {
            return DependencyProperty.Register(propertyName, typeof(TProp), typeof(T), new(defaultValue));
        }

        /// <summary>
        /// Registers a Dependency property with change callback to void [PropertyName]_Changed(TProp newVal)
        /// </summary>
        /// <typeparam name="TProp">Type of the property</typeparam>
        /// <param name="propertyName">Name of the property</param>
        /// <param name="defaultValue">Default value of the property</param>
        /// <returns>The registered dependency property</returns>
        public DependencyProperty RegisterWithChangeCallback<TProp>( string propertyName, TProp defaultValue )
        {
            var changeCallbackMethodInfo = GetChangeCallbackMethodInfo(propertyName, typeof(TProp));
            void OnChangeCallback(DependencyObject obj, DependencyPropertyChangedEventArgs arg)
            {
                changeCallbackMethodInfo.Invoke(obj, new[] { arg.NewValue });
            }
            return RegisterWithChangeCallback<TProp>(propertyName, defaultValue, OnChangeCallback);
        }

        /// <summary>
        /// Registers a Dependency property with the provided change callback
        /// </summary>
        /// <typeparam name="TProp">Type of the property</typeparam>
        /// <param name="propertyName">Name of the property</param>
        /// <param name="defaultValue">Default value of the property</param>
        /// <param name="callback">Callback to invoke on change</param>
        /// <returns>The registered dependency property</returns>
        public DependencyProperty RegisterWithChangeCallback<TProp>(string propertyName, TProp defaultValue, PropertyChangedCallback callback)
        {
            return DependencyProperty.Register(propertyName, typeof(TProp), typeof(T), new(defaultValue, callback));
        }

        private MethodInfo GetChangeCallbackMethodInfo(string propertyName, Type propType)
        {
            var ownerType = typeof(T);
            var methodName = $"{propertyName}_Changed";
            var methodArgs = new[] { propType };
            var methodBindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

            MethodInfo? methodInfo = ownerType.GetMethod(methodName, methodBindFlags, methodArgs);
            if (methodInfo != null)
            {
                return methodInfo;
            }
            throw new Exception($"{ownerType.Name} does not implement method {methodName}({propType.Name})");
        }
    }
}

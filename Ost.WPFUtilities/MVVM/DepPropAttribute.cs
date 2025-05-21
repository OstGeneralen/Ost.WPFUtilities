using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Ost.WpfUtils.MVVM
{
    internal static class DepProps
    {
        internal static void RegisterForAssembly(Assembly asm)
        {
            var asmTypes = asm.GetTypes();
            foreach(var t in asmTypes)
            {
                RegisterDependencyProperties(t);
            }
        }

        static Action<DependencyProperty> GetInstanceSetter(Type onType, PropertyInfo depPropAttributeProperty,  string instanceName )
        {
            var asProperty = onType.GetProperty(instanceName, BindingFlags.Static | BindingFlags.Public);
            if (asProperty != null)
            {
                return (depProp) => asProperty.SetValue(null, depProp);
            }

            var asField = onType.GetField(instanceName, BindingFlags.Static | BindingFlags.Public);
            if(asField != null)
            {
                return (depProp) => asField.SetValue(null, depProp);
            }

            throw new Exception($"No matching static DependencyProperty instance for property {depPropAttributeProperty.Name} in type {onType.Name}");
        }

        public static void RegisterDependencyProperties( Type forType )
        {
            foreach (var property in forType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                var depPropAttribute = property.GetCustomAttribute<DepProp>();
                if (depPropAttribute == null || !depPropAttribute.ShouldAutoRegister) continue; // Skip if not a DepProp marked property or auto register is disabled

                // Generate the metadata for the DependencyProperty (default value and/or change callback)
                var propertyMetadata = depPropAttribute.GenerateMetadata(forType, property);

                // Register DependencyProperty and assign to the static instance in the type
                var registered = DependencyProperty.Register(property.Name, property.PropertyType, forType, propertyMetadata);
                var depPropInstanceName = depPropAttribute.GetDependencyPropertyInstanceName(property);
                var depPropInstanceSetter = GetInstanceSetter(forType, property, depPropInstanceName);
                depPropInstanceSetter(registered);

                Debug.WriteLine($"Registered dependency property T: {forType.Name} | P: {property.PropertyType.Name} {property.Name} | DP: {depPropInstanceName}");
            }
        }
    }

    [Flags]
    public enum EDepPropFlags
    {
        None = 0,
        ManualRegister = 1 << 0,
        WithChangeCallback = 1 << 1,
        Default = None,
    }

    public class DepProp : Attribute
    {
        public EDepPropFlags Flags { get; private set; }
        public object? DefaultValue { get; private set; }
        
        internal string CustomPropertyInstanceName { get; private set; }
        internal bool HasCustomName { get; private set; }
        internal bool IsNullable { get; private set; }

        internal bool ShouldAutoRegister => !Flags.HasFlag(EDepPropFlags.ManualRegister);
        internal bool HasChangeCallback => Flags.HasFlag(EDepPropFlags.WithChangeCallback);

        public DepProp(EDepPropFlags flags = EDepPropFlags.Default, string customDepPropName = "")
            : this(flags, null, false, customDepPropName)
        {
        }
        public DepProp(object defaultValue, EDepPropFlags flags = EDepPropFlags.Default, string customDepPropName = "")
            : this(flags, defaultValue, false, customDepPropName)
        {
        }
        protected DepProp(EDepPropFlags flags, object? defVal, bool nullable, string customName)
        {
            DefaultValue = defVal;
            Flags = flags;
            IsNullable = nullable;
            HasCustomName = string.IsNullOrEmpty(customName) == false;
            CustomPropertyInstanceName = customName;
        }

        internal string GetDependencyPropertyInstanceName(PropertyInfo ownerProperty)
        {
            if (HasCustomName) return CustomPropertyInstanceName;
            return $"{ownerProperty.Name}Property";
        }
        internal PropertyMetadata GenerateMetadata(Type ownerType, PropertyInfo ownerProperty)
        {
            var metadata = new PropertyMetadata(DefaultValue);
            if (!IsNullable && DefaultValue == null)
            {
                metadata = new PropertyMetadata();
            }

            if(HasChangeCallback)
            {
                metadata.PropertyChangedCallback = GenerateCallback(ownerType, ownerProperty);
            }

            return metadata;

        }
        internal PropertyChangedCallback GenerateCallback(Type ownerType, PropertyInfo ownerProperty)
        {
            string changeCallbackMethodName = $"{ownerProperty.Name}_Changed";
            var methodBindFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            var propType = ownerProperty.PropertyType;

            // void Name_Changed(T new) version callback
            var methodArgs = new[] { propType };
            var method = ownerType.GetMethod(changeCallbackMethodName, methodBindFlags, methodArgs);
            if (method != null)
            {
                return new PropertyChangedCallback((o, a) => { method.Invoke(o, new[] { a.NewValue }); });
            }
            
            // void Name_Changed(T old, T new) version callback
            methodArgs = new[] { propType, propType };
            method = ownerType.GetMethod(changeCallbackMethodName, methodBindFlags, methodArgs);
            if ( method != null)
            {
                return new PropertyChangedCallback((o, a) => { method.Invoke(o, new[] { a.OldValue, a.NewValue }); });
            }

            throw new Exception($"WithChangeCallback flag set for DepProp {ownerProperty.Name} but no matching {changeCallbackMethodName}({propType.Name}) OR {changeCallbackMethodName}({propType.Name}, {propType.Name}) callback found.");
        }
    }

    public class NullableDepProp : DepProp
    {
        public NullableDepProp(EDepPropFlags flags = EDepPropFlags.Default, object? defaultValue = null, string customDepPropName = "")
            : base(
                  flags, 
                  defaultValue, 
                  true,
                  customDepPropName
                  )
        {}
    }
}

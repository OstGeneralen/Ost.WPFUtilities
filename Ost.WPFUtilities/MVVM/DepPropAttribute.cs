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
        public static void RegisterForAssembly(Assembly asm)
        {
            var asmTypes = asm.GetTypes();
            foreach(var t in asmTypes)
            {
                RegisterDependencyProperties(t);
            }
        }

        public static void RegisterDependencyProperties( Type forType )
        {
            var depProps_Property = forType.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Where(m => m.GetCustomAttribute<DepProp>() != null);


            foreach (var property in depProps_Property)
            {
                var attribute = property.GetCustomAttribute<DepProp>();
                if (attribute == null) continue;

                if (attribute.DepPropType.HasFlag(EDepPropType.CustomRegister)) continue; // Skip as user has told us that they wish to custom register this dependency property

                var dependencyPropertyInstanceName = $"{property.Name}Property";
                var dependencyProperty = forType.GetProperty(dependencyPropertyInstanceName, BindingFlags.Static | BindingFlags.Public);
                var dependencyPropertyMember = forType.GetField(dependencyPropertyInstanceName, BindingFlags.Static | BindingFlags.Public);

                var setPropertyCall = (DependencyProperty value) =>
                    {
                        if (dependencyProperty != null) dependencyProperty.SetValue(null, value);
                        else if (dependencyPropertyMember != null) dependencyPropertyMember.SetValue(null, value);
                        else throw new Exception();
                    };

                if (dependencyProperty == null && dependencyPropertyMember == null) throw new Exception($"No matching static DependencyProperty instance for property {property.Name} in type {forType.Name}");


                PropertyMetadata propertyMetadata = new PropertyMetadata(attribute.DefaultValue);

                bool isDefaultValueNull = attribute.DefaultValue == null;
                bool isNullable = attribute.Nullable;
                if(!isNullable && isDefaultValueNull)
                {
                    // If the value isn't nullable we're not setting the default value since this will cause type errors
                    propertyMetadata = new PropertyMetadata();
                }

                if (attribute.DepPropType.HasFlag(EDepPropType.OnChangeCallback))
                {
                    var callbackMethodName = $"{property.Name}_Changed";
                    var callbackMethodArgs = new Type[] { property.PropertyType };
                    var callbackMethod = forType.GetMethod(callbackMethodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                    if (callbackMethod == null) throw new Exception($"OnChangeCallback flag set for DepProp {property.Name} but no method {callbackMethodName}({property.PropertyType.Name}) implemented");

                    propertyMetadata.PropertyChangedCallback = (o, a) => callbackMethod.Invoke(o, new[] { a.NewValue });
                }

                var registered = DependencyProperty.Register(property.Name, property.PropertyType, forType, propertyMetadata);
                setPropertyCall(registered);

                string dpName = dependencyProperty != null ? dependencyProperty.Name : dependencyPropertyMember != null ? dependencyPropertyMember.Name : "";

                Debug.WriteLine($"Registered dependency property T: {forType.Name} | P: {property.PropertyType.Name} {property.Name} | DP: {dpName}");
            }
        }
    }

    [Flags]
    public enum EDepPropType
    {
        None = 0,
        CustomRegister = 1 << 0,
        OnChangeCallback = 1 << 1,
        Default = None,
    }

    public class DepProp : Attribute
    {
        public EDepPropType DepPropType { get; private set; }
        public object? DefaultValue { get; private set; }
        public bool Nullable { get; protected set; }
        public DepProp(EDepPropType t = EDepPropType.Default, object? defaultValue = null)
            : this(t, defaultValue, false)
        {
        }

        protected DepProp(EDepPropType t, object? defVal, bool nullable)
        {
            DepPropType = t;
            DefaultValue = defVal;
            Nullable = nullable;
        }
    }

    public class NullableDepProp : DepProp
    {
        public NullableDepProp(EDepPropType t = EDepPropType.Default, object? defaultValue = null)
            : base(t, defaultValue, true)
        {
        }
    }
}

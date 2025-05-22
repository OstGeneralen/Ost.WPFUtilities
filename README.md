Utilities and helpers to reduce required boilerplate and repetetiveness when writing apps using [Windows Presentation Foundation](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/overview/).

## MVVM Workflow Utilities
### `Ost.WpfUtils.MVVM.ViewModel`
Base type for View Models with [one-line setter utility](#one-line-property-setter) for owned properties

### `Ost.WpfUtils.MVVM.View`
Base type for Views (this is the base class of i.e your user controls)
* Has the [one-line setter utility](#one-line-property-setter) for owned normal properties
* Can use `DependencyProperty` [auto registration](#dependency-property-auto-registration)

---

## One Line Property Setter
In types derived from either `View` or `ViewModel`
```csharp
// Without
public class PropertyOwner : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    public PropertyType PropertyName
    {
        get => _propertyName;
        set
        {
            _propertyName = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PropertyName));
        }
    }
    private PropertyType _propertyName;
}

// With
public class PropertyOwner : ViewModel // Or View
{
    public PropertyType PropertyName
    {
        get => _propertyName;
        set => PropertySet(ref _propertyName, value, nameof(PropertyName));
    }
    private PropertyType _propertyName;
}
```

## Dependency Property Auto Registration
```csharp
public class PropertyOwner : View
{
    public static DependencyProperty? PropNameProperty;
    [DepProp] public PropertyType PropName
    {
        get => (PropertyType)GetValue(PropNameProperty);
        set => SetValue(PropNameProperty);
    }

    static PropertyOwner() => StaticInitializeView<PropertyOwner>();
}
```
### Auto-Registration Requirements
* The `DependencyProperty` instance name must follow the pattern `[PropertyName]Property`
    * If you wish to use a custom name, this must be provided to the `DepProp` attribute as `DepProp(customDepPropName: [name]`
* The type must have a static constructor that invokes the `StaticInitializeView<Type>()` method
    * This method is responsible for actually performing the auto registration
 
### The DepProp Attribute options
* You can opt out of auto-registration by providing the attribute with the flag `EDepPropFlags.ManualRegister`
* You can have an on-change callback for your property by providing the attribute with `EDepPropFlags.WithChangeCallback`
    * Callback __must__ be implemented as either of the two options:
        * `void [PropertyName]_Changed(PropertyType newValue)`
        * `void [PropertyName]_Changed(PropertyType oldValue, PropertyType newValue)`
        * If none of the above is found when you set the `WithChangeCallback` flag an exception will be thrown
* You can provide a default value for the dependency property to the attribute
* If your attribute is __nullable__, you should use the `NullableDepProp` attribute instead
     
          

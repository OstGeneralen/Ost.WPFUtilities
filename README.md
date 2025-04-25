# Ost.WpfUtils
A simple library of utilities for building applications using WPF and .NET 9.

This does not aim to change any behaviours of the Wpf workflow, instead wrapping a bunch of multi-parameter calls behind convenience types and interfaces that deals with it for you.

__Without WpfUtils__
```csharp
public class Example : UserControl, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public static readonly DependencyProperty FooProperty;
    public string Foo
    {
        get => (string)GetValue(FooProperty);
        set => SetValue(FooProperty, value);
    }

    public static readonly DependencyProperty BarProperty;
    public int Bar
    {
        get => (int)GetValue(BarProperty);
        set => SetValue(BarProperty, value);
    }

    public float NormalProperty
    {
        get => _normalProperty;
        set
        {
            _normalProperty = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(NormalProperty)));
        }
    }
    private float _normalProperty = 42.0f;

    static Example()
    {
        // Property with no change callback
        FooProperty = DependencyProperty.Register(
            nameof(Foo), 
            typeof(string), 
            typeof(Example), 
            new PropertyMetadata(""));

        // Property with change callback
        BarProperty = DependencyProperty.Register(
            nameof(Bar),
            typeof(int),
            typeof(Example),
            new PropertyMetadata(0, OnBarChanged));

        
    }
    static void OnBarChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
        ((Example)obj).Bar_Changed((int)args.NewValue);
    }

    void Bar_Changed(int newValue) { /*Handle change*/ }
}
```
__With WpfUtils__
```csharp
public class Example : View // Or ViewModel
{
    public static readonly DependencyProperty FooProperty;
    public string Foo
    {
        get => (string)GetValue(FooProperty);
        set => SetValue(FooProperty, value);
    }

    public static readonly DependencyProperty BarProperty;
    public int Bar
    {
        get => (int)GetValue(BarProperty);
        set => SetValue(BarProperty, value);
    }

    public float NormalProperty
    {
        get => _normalProperty;
        set => PropertySet(ref _normalProperty, value, nameof(NormalProperty));
    }
    private float _normalProperty = 42.0f;

    static Example()
    {
        var registrator = new DependencyPropertyRegistrator<Example>();

        // Property with no change callback
        FooProperty = registrator.Register(nameof(Foo), "");

        // Property with change callback
        BarProperty = registrator.RegisterWithChangeCallback(nameof(Bar), 0);

    }

    // No need for the static invoke with casts, this method is automatically found and invoked
    void Bar_Changed(int newValue) { /*Handle change*/ }
}
```


## Included Utilities
### RelayCommand
Utility command type that allows you to specify in code on construction the behaviour of both Execute and CanExecute.
```csharp
ICommand ExampleCommand
{
  get
  {
    if(_exampleCommand == null)
    {
      _exampleCommand = new RelayCommand(ExampleCmdAction, ExampleCmdCanExecute);
    }
    return _exampleCommand;
  }
}
RelayCommand? _exampleCommand = null;

void ExampleCmdAction() => /*Your cmd action logic*/;
bool ExampleCmdCanExecute() => return /*Your evaluation to bool here*/;
```

### DependencyPropertyRegistrator
Utility to help deal with registration of Dependency Properties in a slightly cleaner way.

##### Default Version
```csharp
class ExampleType : UserControl
{
  public int Value
  {
    get => (int)GetValue(ValueProperty);
    set => SetValue(ValueProperty, value);
  }

  public static readonly DependencyProperty ValueProperty; 

  static ExampleType()
  {
    ValueProperty = DependencyProperty.Register(nameof(Value), typeof(int), typeof(ExampleType), new(0));
  }
```

##### Ost.WpfUtilities Version

```csharp
class ExampleType : UserControl
{
  public int Value
  {
    get => (int)GetValue(ValueProperty);
    set => SetValue(ValueProperty, value);
  }

  public static readonly DependencyProperty ValueProperty;

  static ExampleType()
  {
    var registrator = new DependencyPropertyRegistrator<ExampleType>();
    ValueProperty = registrator.Register(nameof(Value), 0);
  }
}
```
In addition to the cleaned up registration interface, it allows registering with an automatically found callback method for when the property is changed!
```csharp
class ExampleType : UserControl
{
  public int Value
  {
    get => (int)GetValue(ValueProperty);
    set => SetValue(ValueProperty, value);
  }
  public static readonly DependencyProperty ValueProperty;

  void Value_Changed(int newValue) { /*Logic to update based on change here*/ }

  static ExampleType()
  {
    var registrator = new DependencyPropertyRegistrator<ExampleType>();
    ValueProperty = registrator.RegisterWithChangeCallback(nameof(Value), 0);
  }
}
```
> [!note]
> The `WithChangeCallback` registration will look for a method named `[PropertyName]_Changed` which takes a single argument of the property type (new value).
> If this method is not found, an exception will be thrown. This method __can be__ `private`.
>
> `RegisterWithChangeCallback<TProp>(string, TProp, PropertyChangedCallback)` can be used to provide your own callback instead of relying on the reflection based version.

### View and ViewModel base types
Base types for your ViewModels and Views (`UserControl`) which contains simple helpers to notify property changes and one line setting with notifying.

##### Default Version
```csharp
// For default user controls
class Example : UserControl, INotifyPropertyChanged
{
  public event PropertyChangedEventHandler? PropertyChanged;

  int Value
  {
    get => _value;
    set
    {
      _value = Value;
      PropertyChanged?.Invoke(this, new(nameof(Value));
    }
  }
  int _value = 0;
}
```

##### Ost.WpfUtilities Notify Method
```csharp
class Example : View // Or ViewModel
{
  // Set and notify using provided function (this example has the same behaviour as the one line version)
  // Provided to allow you to have custom logic on set
  public int Value
  {
    get => _value;
    set
    {
      _value = value;
      NotifyPropChanged(nameof(Value));
    }
  }
  private int _value = 0;
```

##### Ost.WpfUtilities OneLine Version
```csharp
class Example : View // Or ViewModel
{
  // One line set and notify
  public int Value
  {
    get => _value;
    set => PropertySet(ref _value, value, nameof(Value));
  }
  private int _value = 0;
}
```



using Ost.WpfUtils.MVVM;
using System.Diagnostics;
using System.Windows;

namespace Ost.WpfUtils.ExampleApp
{
    /// <summary>
    /// Interaction logic for SampleDependencyPropertyView.xaml
    /// </summary>
    public partial class SampleDependencyPropertyView : View
    {
        public static DependencyProperty? TextContentProperty;
        [DepProp(EDepPropType.Default, "")] public string TextContent
        {
            get => (string)GetValue(TextContentProperty);
            set => SetValue(TextContentProperty, value);
        }

        static SampleDependencyPropertyView() => StaticInitializeView<SampleDependencyPropertyView>();
        public SampleDependencyPropertyView()
        {
            InitializeComponent();
        }
    }
}

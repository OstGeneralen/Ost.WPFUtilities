using Ost.WpfUtils.MVVM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Ost.WpfUtils.ExampleApp
{
    /// <summary>
    /// Interaction logic for SampleDependencyPropertyView.xaml
    /// </summary>
    public partial class SampleDependencyPropertyView : View
    {
        public static readonly DependencyProperty TextContentProperty;
        public string TextContent
        {
            get => (string)GetValue(TextContentProperty);
            set => SetValue(TextContentProperty, value);
        }

        static SampleDependencyPropertyView()
        {
            // Registering dependency property using the DependencyPropertyRegistrator
            var registrator = new DependencyPropertyRegistrator<SampleDependencyPropertyView>();
            TextContentProperty = registrator.Register(nameof(TextContent), "No text");
        }

        public SampleDependencyPropertyView()
        {
            InitializeComponent();
        }
    }
}

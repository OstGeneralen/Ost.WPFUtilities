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

namespace Ost.WpfUtils.Controls
{
    /// <summary>
    /// Interaction logic for LabeledPanel.xaml
    /// </summary>
    public partial class LabeledPanel : View
    {
        public static DependencyProperty? PanelContentProperty;
        public static DependencyProperty? PanelLabelProperty;
        public static DependencyProperty? PanelBackgroundProperty;
        public static DependencyProperty? CornerRadiusProperty;

        [NullableDepProp] public UIElement? PanelContent
        {
            get => (UIElement?)GetValue(PanelContentProperty);
            set => SetValue(PanelContentProperty, value);
        }
        [DepProp(defaultValue: "Unnamed")] public string PanelLabel
        {
            get => (string)GetValue(PanelLabelProperty);
            set => SetValue(PanelLabelProperty, value);
        }
        [DepProp] public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }
        [DepProp] public Brush PanelBackground
        {
            get => (Brush)GetValue(PanelBackgroundProperty);
            set => SetValue(PanelBackgroundProperty, value);
        }

        static LabeledPanel() => StaticInitializeView<LabeledPanel>(); 
        public LabeledPanel()
        {
            InitializeComponent();
            PART_Body.DataContext = this;
        }

        private void View_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if((bool)e.NewValue)
            {
                PART_Body.Opacity = 1;
            }
            else
            {
                PART_Body.Opacity = 0.5;
            }
        }
    }
}

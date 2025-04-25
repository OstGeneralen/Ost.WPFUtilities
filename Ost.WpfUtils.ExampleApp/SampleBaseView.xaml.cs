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
    /// Interaction logic for SampleBaseView.xaml
    /// </summary>
    public partial class SampleBaseView : View
    {
        public SampleViewModel TargetViewModel
        {
            get => _targetViewModel != null ? _targetViewModel : throw new Exception("Target was null");
            set => PropertySet(ref _targetViewModel, value, nameof(TargetViewModel)); // Convenience one line set from View base class
        }
        private SampleViewModel? _targetViewModel;

        public SampleBaseView()
        {
            InitializeComponent();
        }
    }
}

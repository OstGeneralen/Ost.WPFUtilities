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
    /// Interaction logic for DoubleBox.xaml
    /// </summary>
    public partial class DoubleBox : View
    {
        public double Value
        {
            get => _value;
            set => PropertySet(ref _value, value, nameof(Value), Value_Changed);
        }
        private double _value = 0.0;
        private void Value_Changed(double newVal)
        {
            if(Value < Min || Value > Max)
            {
                Value = Math.Clamp(Value, Min, Max);
            }
            else
            {
                PART_TextInput.Text = Value.ToString();
            }
        }

        public double Min
        {
            get => _min;
            set => PropertySet(ref _min, value, nameof(Min));
        }
        private double _min = 0.0;

        public double Max
        {
            get => _max;
            set => PropertySet(ref _max, value, nameof(Max));
        }
        private double _max = double.MaxValue;

        public bool IncrementDecrementButtons
        {
            get => _incrementDecrementButtons;
            set => PropertySet(ref _incrementDecrementButtons, value, nameof(IncrementDecrementButtons), IncrementDecrementButtons_Changed);
        }
        private bool _incrementDecrementButtons = true;
        private void IncrementDecrementButtons_Changed(bool newVal)
        {
            if (newVal) PART_Buttons.Visibility = Visibility.Visible;
            else PART_Buttons.Visibility = Visibility.Collapsed;
        }

        public double IncrementDecrementStep
        {
            get => _incrementDecrementStep;
            set => PropertySet(ref _incrementDecrementStep, value, nameof(IncrementDecrementStep));
        }
        private double _incrementDecrementStep = 1.0;

        public ICommand IncrementCommand { get; private set; }
        public ICommand DecrementCommand { get; private set; }

        public DoubleBox()
        {
            InitializeComponent();
            PART_Host.DataContext = this;
            IncrementCommand = new RelayCommand(() => Value = Value + IncrementDecrementStep, () => Value < Max);
            DecrementCommand = new RelayCommand(() => Value = Value - IncrementDecrementStep, () => Value > Min);
        }

        private void PART_TextInput_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            bool hasDecimalPoint = PART_TextInput.Text.Contains('.');

            foreach(var c in e.Text)
            {
                if(c == '.')
                {
                    if(hasDecimalPoint)
                    {
                        e.Handled = true;
                        return;
                    }
                }
                else if(!char.IsDigit(c))
                {
                    e.Handled = true;
                    return;
                }
            }
        }

        private void PART_TextInput_LostFocus(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrEmpty(PART_TextInput.Text))
            {
                Value = Min;
            }
            else if(double.TryParse(PART_TextInput.Text, out double val))
            {
                Value = val;
            }
        }
    }
}

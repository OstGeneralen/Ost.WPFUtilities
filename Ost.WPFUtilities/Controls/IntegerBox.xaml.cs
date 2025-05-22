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
using Ost.WpfUtils.MVVM;

namespace Ost.WpfUtils.Controls
{
    /// <summary>
    /// Interaction logic for IntegerBox.xaml
    /// </summary>
    public partial class IntegerBox : View
    {
        public int Value
        {
            get => _value;
            set => PropertySet(ref _value, value, nameof(Value), Value_Changed);
        }
        private int _value = 0;
        void Value_Changed(int newVal)
        {
            if(newVal > Max || newVal < Min)
            {
                Value = Math.Clamp(newVal, Min, Max);
            }
            else
            {
                PART_Input.Text = newVal.ToString();
            }
        }

        public int Max 
        {
            get => _max;
            set => PropertySet(ref _max, value, nameof(Max));
        }
        private int _max = int.MaxValue;

        public int Min
        {
            get => _min;
            set => PropertySet(ref _min, value, nameof(Min));
        }
        private int _min = 0;

        public bool IncrementDecrementButtons
        {
            get => _incrementDecrementButtons;
            set => PropertySet(ref _incrementDecrementButtons, value, nameof(IncrementDecrementButtons), IncrementDecrementButtons_Changed);
        }
        private bool _incrementDecrementButtons;

        public int IncrementDecrementStep { get; set; } = 1;

        void IncrementDecrementButtons_Changed(bool newVal)
        {
            if (newVal) PART_Buttons.Visibility = Visibility.Visible;
            else PART_Buttons.Visibility = Visibility.Collapsed;
        }

        public ICommand IncrementCommand { get; private set; }
        public ICommand DecrementCommand { get; private set; }

        public IntegerBox()
        {
            InitializeComponent();
            PART_Host.DataContext = this;

            IncrementCommand = new RelayCommand(() => Value = Value + IncrementDecrementStep, () => Value < Max);
            DecrementCommand = new RelayCommand(() => Value = Value - IncrementDecrementStep, () => Value > Min);
        }

        private void PART_Input_LostFocus(object sender, RoutedEventArgs e)
        {
            if(string.IsNullOrEmpty(PART_Input.Text))
            {
                Value = 0;
            }
            else if(int.TryParse(PART_Input.Text, out int result))
            {
                Value = result;
            }
        }
        private void PART_Input_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            foreach(var c in e.Text)
            {
                if(!char.IsDigit(c))
                {
                    e.Handled = true;
                    return;
                }
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Ost.WpfUtils.MVVM;

namespace Ost.WpfUtils.ExampleApp
{
    // Inheriting from View Model base type
    public class SampleViewModel : ViewModel
    {
        public string TextValue
        {
            get => _textValue;
            set => PropertySet(ref _textValue, value, nameof(TextValue)); // Convenience one line setter from ViewMode
        }
        private string _textValue = "No Text";

        public string AssignedTextValue
        {
            get => _assignedTextValue;
            set => PropertySet(ref _assignedTextValue, value, nameof(AssignedTextValue)); // Convenience one line setter from ViewMode
        }
        private string _assignedTextValue = "No text assigned";

        // Relay command example using simple lambda
        public ICommand AssignTextValueCommand
        {
            get
            {
                if(_assignTextValueCommand == null )
                {
                    _assignTextValueCommand = new RelayCommand(() => AssignedTextValue = TextValue );
                }
                return _assignTextValueCommand;
            }
        }
        private RelayCommand? _assignTextValueCommand = null;
    }
}

using System.Windows.Input;

namespace Ost.WpfUtils.MVVM
{
    public class RelayCommand : ICommand
    {
        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public RelayCommand(Action<object?> execute, Predicate<object?>? canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }
        public RelayCommand(Action execute, Func<bool> canExecute)
            : this( (o) => execute(), (o) => canExecute() )
        {
        }
        public RelayCommand(Action execute)
            : this((o) => execute())
        {
        }

        public bool CanExecute(object? parameter) => _canExecute != null ? _canExecute.Invoke(parameter) : true;
        public void Execute(object? parameter) => _execute.Invoke(parameter);

        private Action<object?> _execute;
        private Predicate<object?>? _canExecute;
    }
}

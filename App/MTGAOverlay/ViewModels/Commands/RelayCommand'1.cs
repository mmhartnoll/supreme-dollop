using MindSculptor.Tools;
using System;
using System.Windows.Input;

namespace MindSculptor.App.MtgaOverlay.ViewModels.Commands
{
    internal class RelayCommand<TParameter> : ICommand
    {
        private readonly Action<TParameter> execute;
        private readonly NullableReference<Func<TParameter, bool>> canExecute;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RelayCommand(Action<TParameter> execute)
            : this(execute, null) { }

        public RelayCommand(Action<TParameter> execute, NullableReference<Func<TParameter, bool>> canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
            => !canExecute.HasValue || canExecute.Value((TParameter)parameter);

        public void Execute(object parameter)
            => execute((TParameter)parameter);
    }
}

using MindSculptor.Tools;
using System;
using System.Windows.Input;

namespace MindSculptor.App.MtgaOverlay.ViewModels.Commands
{
    internal class RelayCommand : ICommand
    {
        private readonly Action execute;
        private readonly NullableReference<Func<bool>> canExecute;

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RelayCommand(Action execute)
            : this(execute, null) { }

        public RelayCommand(Action execute, NullableReference<Func<bool>> canExecute)
        {
            this.execute = execute;
            this.canExecute = canExecute;
        }

        public bool CanExecute(object _)
            => !canExecute.HasValue || canExecute.Value();

        public void Execute(object _)
            => execute();
    }
}

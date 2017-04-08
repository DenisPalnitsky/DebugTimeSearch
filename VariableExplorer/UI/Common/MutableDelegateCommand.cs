using System;
using System.Windows.Input;

namespace SearchLocals.UI.Common
{
    public class MutableDelegateCommand : ICommand
    {
        Action _action;
        public Action Action
        {
            get { return _action; }
            set
            {
                _action = value;
                CanExecuteChanged?.Invoke(this, null);
            }
        }

        public void Execute(object parameter)
        {
            if (Action == null)
                throw new InvalidOperationException("Action is not assigned to delegate");
            Action();
        }

        public bool CanExecute(object parameter)
        {
            return Action != null;
        }

#pragma warning disable 67
        public event EventHandler CanExecuteChanged;
#pragma warning restore 67
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

#pragma warning disable CS0067

namespace reCLI.ViewModel
{
    public class RelayCommand<T> : ICommand
    {
        private Action<T> _action;

        public RelayCommand(Action<T> action)
        {
            _action = action;
        }

        public event EventHandler CanExecuteChanged;

        public virtual void Execute(object parameter)
        {
            Execute((T)parameter);
        }

        public virtual void Execute(T parameter)
        {
            _action?.Invoke(parameter);
        }

        public virtual bool CanExecute(object parameter)
        {
            return true;
        }
    }

    public class RelayCommand : ICommand
    {
        private Action _action;

        public RelayCommand(Action action)
        {
            _action = action;
        }

        public virtual bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged;

        public virtual void Execute(object parameter)
        {
            _action?.Invoke();
        }
    }
}

#pragma warning restore CS0067
using System;
using System.Collections.Generic;
using System.Text;

namespace WordbookImpressLibrary.Helper
{
    public class DelegateCommand : System.Windows.Input.ICommand
    {
        public event EventHandler CanExecuteChanged;

        public Func<object, bool> CanExecuteDelegate;
        public Action<object> ExecuteDelegate;

        public void OnCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, new EventArgs());
        }

        public DelegateCommand(Func<object, bool> CanExecuteDelegate, Action<object> ExecuteDelegate)
        {
            this.CanExecuteDelegate = CanExecuteDelegate;
            this.ExecuteDelegate = ExecuteDelegate;
        }

        public bool CanExecute(object parameter)
        {
            return CanExecuteDelegate(parameter);
        }

        public void Execute(object parameter)
        {
            ExecuteDelegate(parameter);
        }
    }

}

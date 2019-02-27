using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Helper
{
  public class RelayCommand : ICommand
  {
    public RelayCommand(Action execute) : this(execute, null) { }
    public RelayCommand(Action execute, Func<Boolean> canExecute)
    {
      if (execute == null)
        throw new ArgumentNullException("execute");

      _execute = execute;
      _canExecute = canExecute;
    }
    #region ICommand Members
    public event EventHandler CanExecuteChanged
    {
      add
      {
        if (_canExecute != null)
          CommandManager.RequerySuggested += value;
      }
      remove
      {
        if (_canExecute != null)
          CommandManager.RequerySuggested -= value;
      }
    }
    public bool CanExecute(object parameter)
    {
      return _canExecute != null ? _canExecute(): true;
    }
    public void Execute(object parameter)
    {
      _execute();
    }
    #endregion

    private readonly Action _execute;
    private readonly Func<Boolean> _canExecute;
  }
}

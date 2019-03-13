using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF.Helper;

namespace WPF.UserControls
{
  public class NumberKeyboardControlViewModel : NotifyPropertyChanged
  {
    public NumberKeyboardControlViewModel()
    {
      this.ButtonCommand = new RelayCommand(UpdateValue);
    }

    private void UpdateValue(object parameter)
    {
      var button = parameter as KeyboardButton;
      if (button == null)
        return;

      try
      {
        switch (button.Key)
        {
          case KeyboardKeys.Pin:
            break;
          case KeyboardKeys.CLR:
            break;
          default:
            try
            {
              button.Handle(_sbInput);
              break;
            }
            finally
            {
              this.InputValue = _sbInput.ToString();
            }
        }
      }
      catch (Exception) { }
    }


    public String InputValue
    {
      get { return _inputValue; }
      set
      {
        if (!object.Equals(_inputValue, value))
        {
          _inputValue = value;
          this.RaisePropertyChanged(() => InputValue);
        }
      }
    }
    public RelayCommand ButtonCommand { get; private set; }

    private readonly StringBuilder _sbInput = new StringBuilder();
    private String _inputValue;
  }
}

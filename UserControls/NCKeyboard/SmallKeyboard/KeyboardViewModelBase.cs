using System;
using System.Collections.Generic;
using System.Linq;
using WPF.Helper;

namespace WPF.UserControls
{
    public class KeyboardViewModelBase : NotifyPropertyChanged
  {
    public KeyboardViewModelBase()
    {
      this.ButtonCommand = new RelayCommand(UpdateValue);
    }
    protected void UpdateValue(object parameter)
    {
      var button = parameter as KeyboardButton;
      if (button == null)
        return;

      try
      {
        switch (button.Key)
        {
          case KeyboardKeys.SWITCH:
            //if (KeyboardSwitched != null)
            //  KeyboardSwitched(this, null);
            break;
          case KeyboardKeys.Close:
            //if (Closed != null)
            //  Closed(this, null);
            break;
          case KeyboardKeys.Pin:
            break;
          case KeyboardKeys.CLR:
            break;
          case KeyboardKeys.Inv:
            break;
          case KeyboardKeys.M2I:
            // update InputValue
            break;
          case KeyboardKeys.I2M:
            // update InputValue
            break;
          case KeyboardKeys.Equal:
          case KeyboardKeys.Enter:
          default:
            try
            {
              button.Handle(_inputList);
              break;
            }
            finally
            {
              this.InputValue = Helpers.ListToValue(_inputList.Select(p => p.Value).ToList());
            }
        }
      }
      catch (Exception) { }
    }

    public RelayCommand ButtonCommand { get; private set; }
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

    

    private String _inputValue;
    private readonly List<InputInfo> _inputList = new List<InputInfo>();
  }
}

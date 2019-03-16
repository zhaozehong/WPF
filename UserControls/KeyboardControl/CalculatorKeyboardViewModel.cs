using System;
using System.Collections.Generic;
using System.Linq;
using WPF.Helper;

namespace WPF.UserControls
{
  public enum KeyboardModes { Number, Calculator }
  public class CalculatorKeyboardViewModel : NotifyPropertyChanged
  {
    public CalculatorKeyboardViewModel()
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
            KeyboardMode = KeyboardMode == KeyboardModes.Number ? KeyboardModes.Calculator : KeyboardModes.Number;
            break;
          case KeyboardKeys.Inv:
            this.IsInverse = !IsInverse;
            break;
          case KeyboardKeys.Close:
            break;
          case KeyboardKeys.Pin:
            break;
          case KeyboardKeys.CLR:
            break;
          case KeyboardKeys.M2I:
            // update InputValue
            break;
          case KeyboardKeys.I2M:
            // update InputValue
            break;
          case KeyboardKeys.Equal:
          // update InputValue
            break;
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

    #region Properties
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
    public KeyboardModes KeyboardMode
    {
      get { return _keyboardMode; }
      set
      {
        if (_keyboardMode != value)
        {
          _keyboardMode = value;
          this.RaisePropertyChanged(nameof(KeyboardMode));
        }
      }
    }
    public Boolean IsInverse
    {
      get { return _isInverse; }
      set
      {
        if (_isInverse != value)
        {
          _isInverse = value;
          this.RaisePropertyChanged(nameof(IsInverse));
        }
      }
    }

    #endregion

    #region Variables
    private readonly List<InputInfo> _inputList = new List<InputInfo>();
    private String _inputValue;
    private KeyboardModes _keyboardMode = KeyboardModes.Number;
    private Boolean _isInverse = false;
    #endregion
  }
}

using System;
using System.Windows;
using System.Windows.Data;

namespace Hexagon.Software.NCGage.UserControls
{
  public class CalculatorKeyboardPopup : KeyboardPopup
  {
    public CalculatorKeyboardPopup()
    {
      if (Keyboard != null)
        Keyboard.SetBinding(CalculatorKeyboardControl.ResetOnCalculationProperty, new Binding("ResetOnCalculation") { Source = this });
    }

    protected KeyboardControl _keyboard;
    protected override KeyboardControl Keyboard
    {
      get
      {
        if (_keyboard == null)
          _keyboard = new CalculatorKeyboardControl();
        return _keyboard;
      }
    }

    #region Dependency Properties

    public bool ResetOnCalculation
    {
      get { return (Boolean)GetValue(ResetOnCalculationProperty); }
      set { SetValue(ResetOnCalculationProperty, value); }
    }
    public static readonly DependencyProperty ResetOnCalculationProperty = DependencyProperty.Register("ResetOnCalculation", typeof(bool), typeof(CalculatorKeyboardPopup), new PropertyMetadata(true));

    #endregion
  }
}

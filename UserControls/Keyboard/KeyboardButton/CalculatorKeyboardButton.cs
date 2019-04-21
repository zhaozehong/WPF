using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Hexagon.Software.NCGage.HelperLib;

namespace Hexagon.Software.NCGage.UserControls
{
  public class CalculatorKeyboardButton : KeyboardButton
  {
    public Boolean Handle(List<InputInfo> recordList, string preValue)
    {
      if (recordList == null)
        return false;

      String value = null;
      switch (this.Key)
      {
        case KeyboardKeys.D_D0:
        case KeyboardKeys.D_D1:
        case KeyboardKeys.D_D2:
        case KeyboardKeys.D_D3:
        case KeyboardKeys.D_D4:
        case KeyboardKeys.D_D5:
        case KeyboardKeys.D_D6:
        case KeyboardKeys.D_D7:
        case KeyboardKeys.D_D8:
        case KeyboardKeys.D_D9:
          value = this.Key.ToString().Last().ToString();
          break;
        case KeyboardKeys.PI:
          value = "PI";
          break;
        case KeyboardKeys.Point:
          value = ".";
          break;

        case KeyboardKeys.O_Add:
          value = "+";
          break;
        case KeyboardKeys.O_Substract:
          value = "-";
          break;
        case KeyboardKeys.O_Multiply:
          value = "*";
          break;
        case KeyboardKeys.O_Divide:
          value = "/";
          break;
        case KeyboardKeys.O_Mod:
          value = "%";
          break;
        case KeyboardKeys.O_Square:
          value = "^";
          break;

        // functions
        case KeyboardKeys.F_Abs:
        case KeyboardKeys.F_Sin:
        case KeyboardKeys.F_Cos:
        case KeyboardKeys.F_Tan:
        case KeyboardKeys.F_ln:
        case KeyboardKeys.F_Sqrt:
        case KeyboardKeys.F_ASin:
        case KeyboardKeys.F_ACos:
        case KeyboardKeys.F_ATan:
        case KeyboardKeys.F_Exp:
          value = String.Format("{0}(", Key.ToString().Substring(2));
          break;
        case KeyboardKeys.LeftBracket:
          value = "(";
          break;
        case KeyboardKeys.RightBracket:
          value = ")";
          break;

        case KeyboardKeys.Backspace:
        case KeyboardKeys.C:
          var lastValue = recordList.LastOrDefault();
          if (lastValue != null)
          {
            if (lastValue.Value == "2" && lastValue.Previous != null && lastValue.Previous.Value == "^")
              recordList.RemoveAt(recordList.Count - 1);
            recordList.RemoveAt(recordList.Count - 1);
          }
          break;
        case KeyboardKeys.AC:
          recordList.Clear();
          break;
        case KeyboardKeys.M2I:
        case KeyboardKeys.I2M:
          recordList.Add(new InputInfo(recordList.LastOrDefault(), this.Key, preValue));
          return true;
        case KeyboardKeys.None:
        case KeyboardKeys.Enter:
        case KeyboardKeys.Equal:
        case KeyboardKeys.SWITCH:
        case KeyboardKeys.Pin:
        case KeyboardKeys.CLR:
        case KeyboardKeys.Inv:

        case KeyboardKeys.Close:
          break;
      }
      value = String.Format("{0}{1}", preValue, value);
      if (!String.IsNullOrWhiteSpace(value))
        recordList.Add(new InputInfo(recordList.LastOrDefault(), this.Key, value));
      if (this.Key == KeyboardKeys.O_Square)
        recordList.Add(new InputInfo(recordList.LastOrDefault(), KeyboardKeys.D_D2, "2"));

      return true;
    }

    #region Dependency Properties
    public KeyboardKeys Key
    {
      get { return (KeyboardKeys)GetValue(KeyboardKeyProperty); }
      set { SetValue(KeyboardKeyProperty, value); }
    }
    public static readonly DependencyProperty KeyboardKeyProperty = DependencyProperty.Register("KeyboardKey", typeof(KeyboardKeys), typeof(CalculatorKeyboardButton), new PropertyMetadata(KeyboardKeys.None));

    #endregion
  }
  public class InputInfo
  {
    public InputInfo(InputInfo previous, KeyboardKeys key, String value)
    {
      this.Previous = previous;
      this.Key = key;
      this.Value = value;

      this.IsFunction = KeyboardHelper.IsFunctionKey(this.Key);
      this.IsOperator = KeyboardHelper.IsOperatorKey(this.Key);
      this.IsDigit = KeyboardHelper.IsDigitKey(this.Key);
      this.IsSubmit = KeyboardHelper.IsSubmitKey(this.Key);
      this.IsPoint = KeyboardHelper.IsPointKey(this.Key);
      this.IsLeftBracket = KeyboardHelper.IsLeftBracketKey(this.Key);
      this.IsRightBracket = KeyboardHelper.IsRightBracketKey(this.Key);
      this.IsUnitConverter = KeyboardHelper.IsUnitConverterKey(this.Key);
      this.IsPI = KeyboardHelper.IsPIKey(this.Key);
    }

    public InputInfo Previous { get; private set; }
    public KeyboardKeys Key { get; private set; }
    public String Value { get; private set; }

    public Boolean IsFunction { get; private set; }
    public Boolean IsOperator { get; private set; }
    public Boolean IsDigit { get; private set; }
    public Boolean IsSubmit { get; private set; }
    public Boolean IsPoint { get; private set; }
    public Boolean IsLeftBracket { get; private set; }
    public Boolean IsRightBracket { get; private set; }
    public Boolean IsUnitConverter { get; private set; }
    public Boolean IsPI { get; private set; }
  }
}

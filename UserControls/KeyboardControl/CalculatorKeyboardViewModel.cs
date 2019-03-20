﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Hexagon.Software.NCGage.ExpressionCalculator;
using Hexagon.Software.NCGage.HelperLib;

namespace Hexagon.Software.NCGage.UserControls
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
          case KeyboardKeys.None:
          case KeyboardKeys.Close:
          case KeyboardKeys.Pin:
          case KeyboardKeys.CLR:
          case KeyboardKeys.Enter:
            break;
          case KeyboardKeys.M2I:
            // update InputValue
            break;
          case KeyboardKeys.I2M:
            // update InputValue
            break;

          case KeyboardKeys.Equal:
          default:
            try
            {
              String preValue;
              if (CheckRules(button.Key, out preValue))
                button.Handle(_inputList, preValue);
            }
            finally
            {
              this.InputValue = Helpers.ListToValue(_inputList.Select(p => p.Value).ToList());
              if (button.Key == KeyboardKeys.Equal)
              {
                var convertedInputValue = GetConvertedInputValue();
                var expressionUnit = NumericCalculator.Parse(convertedInputValue);
                this.InputValue = Helpers.GetValueString(expressionUnit != null ? expressionUnit.Compute() : Double.NaN, 0);
                this._inputList.Clear();
              }
            }
            break;
        }
      }
      catch (Exception) { }
    }

    private String GetConvertedInputValue()
    {
      var retValue = Regex.Replace(this.InputValue, " ", "");
      if (String.IsNullOrWhiteSpace(retValue))
        return null;

      int iStart = -1, iEnd = -1;
      var iPow = retValue.IndexOf('^');
      while (iPow != -1)
      {
        // handle string on left
        var strLeft = retValue.Substring(0, iPow);
        if (strLeft.EndsWith(")"))
        {
          iStart = Helpers.GetLastSubExpressionIndex(strLeft);
          if (iStart == -1) return null;  // the input string is an invalid expression

          var sub = strLeft.Substring(0, iStart);
          var functionName = KeyboardHelper.FunctionNames.FirstOrDefault(p => sub.EndsWith(p));
          if (functionName != null)
          {
            iStart -= functionName.Length;
          }
        }
        else
        {
          iStart = iPow;
          if (strLeft.EndsWith("."))
          {
            iStart--;
            strLeft = strLeft.Substring(0, strLeft.Length - 1);
          }

          // must be a numeric value left
          var match = Regex.Match(strLeft, @"\d+$");
          if (!match.Success)
            return null; // the input string is an invalid expression
          iStart -= match.Value.Length;
        }

        var strRight = retValue.Substring(iPow + 1);
        if (strRight.StartsWith("(") || KeyboardHelper.FunctionNames.Any(p => strRight.StartsWith(p)))
        {
          var sub = Helpers.GetSubExpression(strRight, false);
          if (sub == null)
            return null; // the input string is an invalid expression
          iEnd = iPow + sub.Length;
        }
        else // must be numeric value
        {
          var match = Regex.Match(strRight, @"^\d+");
          if (!match.Success)
            return null; // the input string is an invalid expression
          iEnd += match.Value.Length;
        }

        // Replace value
        if (iStart == -1 || iEnd == -1 || iStart == iPow || iEnd == iPow)
          return null; // something wrong

        var x = retValue.Substring(iStart, iPow - iStart);
        var y = retValue.Substring(iPow + 1, iEnd - iPow);
        var newText = String.Format("Pow({0},{1})", x, y);

        retValue = retValue.Substring(0, iStart) + newText + retValue.Substring(iEnd + 1);
        iPow = retValue.IndexOf('^');
      }
      return retValue;
    }
    #region check input rules
    public Boolean CheckRules(KeyboardKeys key, out string preValue)
    {
      preValue = null;
      if (NeedDeleteLastInput(key))
      {
        if (_inputList.Count > 1)
        {
          var llastInput = _inputList[_inputList.Count - 2];
          if (KeyboardHelper.IsOperatorKey(key) && _inputList.Last().Key == KeyboardKeys.O_Substract && (!llastInput.IsDigit && !llastInput.IsRightBracket && !llastInput.IsPoint))
            return false;
        }
        _inputList.RemoveAt(_inputList.Count - 1);
      }
      if (NeedReturn(key))
        return false;

      preValue = null;
      if (NeedAddMultiply(key))
        preValue += "*";
      if (NeedAddZero(key))
        preValue += "0";
      if (NeedAddMultiplyZero(key))
        preValue += "*0";
      if (NeedAddRightBrackets(key))
      {
        var input = this.InputValue;
        var count = input.Split('(').Length - input.Split(')').Length;
        if (count < 0)
          return false;
        if (count > 0)
          preValue += new string(')', count);
      }
      return true;
    }

    private Boolean NeedDeleteLastInput(KeyboardKeys key)
    {
      var lastInput = _inputList.LastOrDefault();
      return lastInput != null && lastInput.IsOperator && (KeyboardHelper.IsOperatorKey(key));
    }
    private Boolean NeedReturn(KeyboardKeys key)
    {
      var lastInput = _inputList.LastOrDefault();
      if ((lastInput == null || lastInput.IsFunction || lastInput.IsLeftBracket) && ((KeyboardHelper.IsOperatorKey(key) && key != KeyboardKeys.O_Substract) || KeyboardHelper.IsEqualKey(key) || KeyboardHelper.IsRightBracketKey(key)))
        return true;

      if (lastInput != null)
      {
        var llastInput = lastInput.Previous;
        if (llastInput != null)
          if (KeyboardHelper.IsOperatorKey(key) && lastInput.Key == KeyboardKeys.O_Substract && (!llastInput.IsDigit && !llastInput.IsRightBracket && !llastInput.IsPoint)) // invalid operation for minus sign
            return true;

        if (lastInput.IsPoint && KeyboardHelper.IsPointKey(key))
          return true;
        if (lastInput.IsOperator && (KeyboardHelper.IsEqualKey(key) || KeyboardHelper.IsRightBracketKey(key)))
          return true;
        if ((lastInput.IsDigit || lastInput.IsPoint || lastInput.IsRightBracket) && KeyboardHelper.IsRightBracketKey(key))
        {
          var leftBracketCount = Helpers.GetCount(this.InputValue, @"[([{]");
          var rightBracketCount = Helpers.GetCount(this.InputValue, @"[)\]}]");
          if (leftBracketCount <= rightBracketCount)
            return true;
        }
      }
      return false;
    }
    private Boolean NeedAddMultiply(KeyboardKeys key)
    {
      var lastInput = _inputList.LastOrDefault();
      if (lastInput == null)
        return false;

      if (lastInput.IsRightBracket && KeyboardHelper.IsDigitKey(key))
        return true;
      return (lastInput.IsDigit || lastInput.IsPoint || lastInput.IsRightBracket) && (KeyboardHelper.IsFunctionKey(key) || KeyboardHelper.IsLeftBracketKey(key));
    }
    private Boolean NeedAddZero(KeyboardKeys key)
    {
      var lastInput = _inputList.LastOrDefault();
      return (lastInput == null || lastInput.IsFunction || lastInput.IsLeftBracket || lastInput.IsOperator) && KeyboardHelper.IsPointKey(key);
    }
    private Boolean NeedAddMultiplyZero(KeyboardKeys key)
    {
      var lastInput = _inputList.LastOrDefault();
      return lastInput != null && lastInput.IsRightBracket && KeyboardHelper.IsPointKey(key);
    }
    private Boolean NeedAddRightBrackets(KeyboardKeys key)
    {
      var lastInput = _inputList.LastOrDefault();
      return lastInput != null && (lastInput.IsDigit || lastInput.IsPoint || lastInput.IsRightBracket) && key == KeyboardKeys.Equal;
    }

    #endregion

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

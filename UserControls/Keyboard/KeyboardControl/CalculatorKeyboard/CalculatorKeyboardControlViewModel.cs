using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Hexagon.Software.NCGage.ExpressionCalculator;
using Hexagon.Software.NCGage.HelperLib;

namespace Hexagon.Software.NCGage.UserControls
{
  public enum KeyboardTypes { Number, Calculator, Full }
  public class CalculatorKeyboardControlViewModel : NotifyPropertyChanged
  {
    public CalculatorKeyboardControlViewModel()
    {
      this.ButtonCommand = new RelayCommand(Update);
    }

    protected void Update(object parameter)
    {
      var button = parameter as CalculatorKeyboardButton;
      if (button == null)
        return;

      try
      {
        switch (button.Key)
        {
          case KeyboardKeys.SWITCH:
            KeyboardType = KeyboardType == KeyboardTypes.Number ? KeyboardTypes.Calculator : KeyboardTypes.Number;
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

          default:
            try
            {
              String preValue;
              if (CheckRules(button.Key, out preValue))
                button.Handle(_inputList, preValue);
            }
            finally
            {
              this.InputValue = ConvertInputListToString();
              if (button.Key == KeyboardKeys.Equal)
              {
                var convertedInputValue = GetConvertedInputValue();
                var expressionUnit = NumericCalculator.Parse(convertedInputValue);
                var computeResult = expressionUnit != null ? expressionUnit.Compute(AngleUnits.Degree) : Double.NaN;
                this.InputValue = Double.IsNaN(computeResult) ? "Error" : computeResult.ToString();
                this._inputList.Clear();
                if (!this.IsResetAfterCalculation)
                  this._inputList.Add(new InputInfo(null, KeyboardKeys.D_Flag, this.InputValue));
              }
            }
            break;
        }
      }
      catch (Exception) { }
    }

    private String ConvertInputListToString()
    {
      try
      {
        var iCurrent = _inputList.FindIndex(p => KeyboardHelper.IsUnitConverterKey(p.Key));
        if (iCurrent == -1)
          return Helpers.ListToValue(_inputList.Select(p => p.Value).ToList());

        int iLast = 0;
        var tempList = new List<String>();
        while (iCurrent != -1)
        {
          tempList.Add(String.Format("{0}({1})", _inputList[iCurrent].Key, Helpers.ListToValue(_inputList.GetRange(iLast, iCurrent - iLast).Select(p => p.Value).ToList())));

          iLast = iCurrent + 1;
          iCurrent = _inputList.FindIndex(iLast, p => KeyboardHelper.IsUnitConverterKey(p.Key));
        }
        return Helpers.ListToValue(tempList);
      }
      catch (Exception)
      {
        return null;
      }
    }
    private String GetConvertedInputValue()
    {
      var retValue = Regex.Replace(this.InputValue, " ", "");
      if (String.IsNullOrWhiteSpace(retValue))
        return null;

      retValue = Regex.Replace(this.InputValue, "PI", Math.PI.ToString());
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
          var match = Regex.Match(strLeft, @"\d+\.?\d*$");
          if (!match.Success)
            return null; // the input string is an invalid expression
          iStart -= match.Value.Length;
        }

        iEnd = iPow;
        var strRight = retValue.Substring(iPow + 1);
        if (strRight.StartsWith("(") || KeyboardHelper.FunctionNames.Any(p => strRight.StartsWith(p)))
        {
          var sub = Helpers.GetSubExpression(strRight, false);
          if (sub == null)
            return null; // the input string is an invalid expression
          iEnd += sub.Length;
        }
        else // must be numeric value
        {
          var match = Regex.Match(strRight, @"^-?\d+\.?\d*$");
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
      if (this.KeyboardType == KeyboardTypes.Number)
        return (key == KeyboardKeys.O_Substract && lastInput != null) || (KeyboardHelper.IsPointKey(key) && this.InputValue.Contains("."));

      if ((lastInput == null || lastInput.IsFunction || lastInput.IsLeftBracket) &&
        ((KeyboardHelper.IsOperatorKey(key) && key != KeyboardKeys.O_Substract) || KeyboardHelper.IsEqualKey(key) || KeyboardHelper.IsRightBracketKey(key) || KeyboardHelper.IsUnitConverterKey(key)))
        return true;

      if (lastInput != null)
      {
        var llastInput = lastInput.Previous;
        if (llastInput != null)
          if (KeyboardHelper.IsOperatorKey(key) && lastInput.Key == KeyboardKeys.O_Substract && (!llastInput.IsDigit && !llastInput.IsRightBracket && !llastInput.IsPoint)) // invalid operation for minus sign
            return true;

        if ((lastInput.IsPoint || GetLastNumericText(this.InputValue).Contains(".")) && KeyboardHelper.IsPointKey(key))
          return true;
        if (lastInput.IsOperator && (KeyboardHelper.IsEqualKey(key) || KeyboardHelper.IsRightBracketKey(key) || KeyboardHelper.IsUnitConverterKey(key)))
          return true;
        if (lastInput.IsUnitConverter && (KeyboardHelper.IsRightBracketKey(key) || KeyboardHelper.IsUnitConverterKey(key)))
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

      if ((lastInput.IsRightBracket || lastInput.IsUnitConverter) && KeyboardHelper.IsDigitKey(key))
        return true;
      return (lastInput.IsDigit || lastInput.IsPoint || lastInput.IsRightBracket || lastInput.IsUnitConverter) && (KeyboardHelper.IsFunctionKey(key) || KeyboardHelper.IsLeftBracketKey(key));
    }
    private Boolean NeedAddZero(KeyboardKeys key)
    {
      var lastInput = _inputList.LastOrDefault();
      return (lastInput == null || lastInput.IsFunction || lastInput.IsLeftBracket || lastInput.IsOperator || lastInput.IsUnitConverter) && KeyboardHelper.IsPointKey(key);
    }
    private Boolean NeedAddMultiplyZero(KeyboardKeys key)
    {
      var lastInput = _inputList.LastOrDefault();
      return lastInput != null && lastInput.IsRightBracket && KeyboardHelper.IsPointKey(key);
    }
    private Boolean NeedAddRightBrackets(KeyboardKeys key)
    {
      var lastInput = _inputList.LastOrDefault();
      return lastInput != null && (lastInput.IsDigit || lastInput.IsPoint || lastInput.IsRightBracket) && (key == KeyboardKeys.Equal || KeyboardHelper.IsUnitConverterKey(key));
    }

    private String GetLastNumericText(String text)
    {
      var match = Regex.Match(text, @"\d+\.?\d*$");
      if (!match.Success)
        return String.Empty;
      return match.Value;
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
    public KeyboardTypes KeyboardType
    {
      get { return _keyboardMode; }
      set
      {
        if (_keyboardMode != value)
        {
          _keyboardMode = value;
          this.RaisePropertyChanged(nameof(KeyboardType));
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
    public bool IsResetAfterCalculation { get; set; } = true;

    #endregion

    #region Variables
    private readonly List<InputInfo> _inputList = new List<InputInfo>();
    private String _inputValue;
    private KeyboardTypes _keyboardMode = KeyboardTypes.Number;
    private Boolean _isInverse = false;
    #endregion
  }
}

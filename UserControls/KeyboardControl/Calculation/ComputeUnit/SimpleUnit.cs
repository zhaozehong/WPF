using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Hexagon.Software.NCGage.HelperLib;

namespace Hexagon.Software.NCGage.ExpressionCalculator
{
  public enum SimpleFunctions { None, Abs, Sqrt, Ceiling, Floor, Truncate, Sign, Log, Log10, Sin, Cos, Tan, Sinh, Cosh, Tanh, Asin, Acos, Atan }
  public class SimpleUnit : ComputeUnit
  {
    public SimpleUnit(String strExpression, SimpleFunctions simpleFunction) : base(strExpression) { this._function = simpleFunction; }

    public override Double Compute()
    {
      if (String.IsNullOrWhiteSpace(this.Expression))
        return Double.NaN;
      if (Helpers.IsNumericValue(this.Expression))
        return this.ApplyFunction(Double.Parse(this.Expression));

      var _operatorList = new List<String>();
      var _unitList = new List<IComputeUnit>();

      try
      {
        // prepare
        int i = 0;
        while (i < Expression.Length)
        {
          var strExpression = Expression.Substring(i);

          var nextMatch = Regex.Match(strExpression, @"[\+\-*/%([{]");
          if (!nextMatch.Success) // must be numeric number
          {
            _unitList.Add(new NumericUnit(strExpression));
            break;
          }

          if (nextMatch.Index == 0)
          {
            if (Regex.IsMatch(nextMatch.Value, @"[([{]"))
            {
              var subExpression = Helpers.GetSubExpression(strExpression);
              var unit = NumericCalculator.Parse(subExpression);
              if (unit == null)
                return Double.NaN;

              _unitList.Add(unit);
              i += subExpression.Length;
            }
            else if (Regex.IsMatch(nextMatch.Value, @"[\+\-*/%]"))
            {
              Debug.Assert(nextMatch.Index == 0 && nextMatch.Length == 1);
              _operatorList.Add(strExpression[0].ToString());
              i++;
            }
          }
          else
          {
            if (Regex.IsMatch(strExpression, @"^[0-9]")) // start with number, it must be a numeric unit
            {
              _unitList.Add(new NumericUnit(strExpression.Substring(0, nextMatch.Index)));
              i += nextMatch.Index;
            }
            else // must be a complex unit
            {
              var subExpression = Helpers.GetSubExpression(strExpression);
              var unit = NumericCalculator.Parse(subExpression);
              if (unit == null)
                return Double.NaN;
              _unitList.Add(unit);
              i += subExpression.Length;
            }
          }
        }

        // calculate
        Debug.Assert(_operatorList.Count == _unitList.Count - 1);
        var operators = new List<String>();
        var values = new List<Double>();
        Double value = Double.NaN;
        for (i = 0; i < _operatorList.Count; i++)
        {
          if (Double.IsNaN(value))
            value = _unitList[i].Compute();
          if (_operatorList[i] == "*" || _operatorList[i] == "/" || _operatorList[i] == "%")
          {
            if (_operatorList[i] == "*")
              value *= _unitList[i + 1].Compute();
            else
            {
              var temp = _unitList[i + 1].Compute();
              if (temp == 0)
                return Double.NaN;

              if (_operatorList[i] == "/")
                value /= temp;
              else
                value %= temp;
            }
            continue;
          }
          if (Double.IsNaN(value))
            return Double.NaN;
          if (operators.Any() && operators.Last() == "-")
            value *= -1;
          values.Add(value);
          operators.Add(_operatorList[i]);

          value = Double.NaN;
        }
        if (Double.IsNaN(value) && (!_operatorList.Any() || _operatorList.Last() == "+" || _operatorList.Last() == "-"))
          value = _unitList.Last().Compute();
        if (!Double.IsNaN(value))
        {
          if (operators.Any() && operators.Last() == "-")
            value *= -1;
          values.Add(value);
        }

        // apply function
        return this.ApplyFunction(values.Sum());
      }
      catch
      {
        return Double.NaN;
      }
      finally
      {
        _operatorList.Clear();
        _unitList.Clear();
      }
    }

    private Double ApplyFunction(Double value)
    {
      switch (_function)
      {
        case SimpleFunctions.None:
          return value;
        case SimpleFunctions.Abs:
          return Math.Abs(value);
        case SimpleFunctions.Sqrt:
          return Math.Sqrt(value);
        case SimpleFunctions.Ceiling:
          return Math.Ceiling(value);
        case SimpleFunctions.Floor:
          return Math.Floor(value);
        case SimpleFunctions.Truncate:
          return Math.Truncate(value);
        case SimpleFunctions.Sign:
          return Math.Sign(value);
        case SimpleFunctions.Log:
          return Math.Log(value);
        case SimpleFunctions.Log10:
          return Math.Log10(value);
        case SimpleFunctions.Sin:
          return Math.Sin(value);
        case SimpleFunctions.Cos:
          return Math.Cos(value);
        case SimpleFunctions.Tan:
          return Math.Tan(value);
        case SimpleFunctions.Sinh:
          return Math.Sinh(value);
        case SimpleFunctions.Cosh:
          return Math.Cosh(value);
        case SimpleFunctions.Tanh:
          return Math.Tanh(value);
        case SimpleFunctions.Asin:
          return Math.Asin(value);
        case SimpleFunctions.Acos:
          return Math.Acos(value);
        case SimpleFunctions.Atan:
          return Math.Atan(value);
      }
      return Double.NaN;
    }

    private SimpleFunctions _function = SimpleFunctions.None;
  }
}

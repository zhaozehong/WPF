using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Hexagon.Software.NCGage.ExpressionCalculator
{
  public enum ComplexFunctions { None = 0, Max, Min, Average, Ave, Sum, Pow, Atan2, LogX, BigMul, IEEERemainder }
  public class ComplexUnit : ComputeUnit
  {
    public ComplexUnit(String strExpression, ComplexFunctions complexFunction) : base(strExpression) { this._function = complexFunction; }
    public override Double Compute(AngleUnits angleUnit)
    {
      if (String.IsNullOrWhiteSpace(Expression))
        return Double.NaN;

      var _unitList = new List<IComputeUnit>();
      try
      {
        var parameters = GetParameterStrings(this.Expression);
        foreach (var parameter in parameters)
        {
          var unit = NumericCalculator.Parse(parameter);
          if (unit == null)
            return Double.NaN;
          _unitList.Add(unit);
        }
        var values = _unitList.Select(p => p.Compute(angleUnit)).ToList();
        return this.ApplyFunction(values, angleUnit);
      }
      catch
      {
        return Double.NaN;
      }
      finally
      {
        _unitList.Clear();
      }
    }

    private List<String> GetParameterStrings(String strParameter)
    {
      if (String.IsNullOrWhiteSpace(strParameter))
        return null;

      var leftIndexs = Regex.Matches(strParameter, @"[([{]").OfType<Match>().Select(m => m.Index).ToList();
      var rightIndexs = Regex.Matches(strParameter, @"[)\]}]").OfType<Match>().Select(m => m.Index).ToList();
      if (leftIndexs.Count != rightIndexs.Count)
        return null;

      var separatorIndexs = Regex.Matches(strParameter, @"[,]").OfType<Match>().Select(m => m.Index).ToList();
      for (int i = separatorIndexs.Count - 1; i >= 0; i--)
      {
        var separatorIndex = separatorIndexs[i];
        if (leftIndexs.Count(p => p < separatorIndexs[i]) != rightIndexs.Count(p => p < separatorIndexs[i]))
          separatorIndexs.RemoveAt(i);
      }
      if (!separatorIndexs.Any())
        return null;

      var tempList = new List<String>();
      tempList.Add(strParameter.Substring(0, separatorIndexs[0]));
      for (int i = 1; i < separatorIndexs.Count; i++)
      {
        tempList.Add(strParameter.Substring(separatorIndexs[i - 1] + 1, separatorIndexs[i] - separatorIndexs[i - 1] - 1));
      }
      tempList.Add(strParameter.Substring(separatorIndexs.Last() + 1));

      return tempList.Where(p => !String.IsNullOrWhiteSpace(p)).ToList();
    }
    private Double ApplyFunction(List<Double> values, AngleUnits angleUnit)
    {
      if (values == null || !values.Any())
        return Double.NaN;

      switch (_function)
      {
        case ComplexFunctions.Min:
          return values.Min();
        case ComplexFunctions.Max:
          return values.Max();
        case ComplexFunctions.Average:
        case ComplexFunctions.Ave:
          return values.Average();
        case ComplexFunctions.Sum:
          return values.Sum();
        case ComplexFunctions.Pow:
          return values.Count >= 2 ? Math.Pow(values[0], values[1]) : Double.NaN;
        case ComplexFunctions.Atan2:
          return GetReturnedAngleValue(values.Count >= 2 ? Math.Atan2(values[0], values[1]) : Double.NaN, angleUnit);
        case ComplexFunctions.LogX:
          return values.Count >= 2 ? Math.Log(values[0], values[1]) : Double.NaN;
        case ComplexFunctions.BigMul:
          return values.Count >= 2 ? Math.BigMul((int)values[0], (int)values[1]) : Double.NaN;
        case ComplexFunctions.IEEERemainder:
          return values.Count >= 2 ? Math.IEEERemainder(values[0], values[1]) : Double.NaN;
      }
      return Double.NaN;
    }

    private ComplexFunctions _function = ComplexFunctions.None;
  }
}

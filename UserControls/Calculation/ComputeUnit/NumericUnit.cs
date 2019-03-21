using System;
using Hexagon.Software.NCGage.HelperLib;

namespace Hexagon.Software.NCGage.ExpressionCalculator
{
  public class NumericUnit : ComputeUnit
  {
    public NumericUnit(String strExpression) : base(strExpression) { }

    public override Double Compute(AngleUnits angleUnit)
    {
      try
      {
        return Helpers.IsNumericValue(this.Expression) ? Double.Parse(this.Expression) : Double.NaN;
      }
      catch
      {
        return Double.NaN;
      }
    }
  }
}

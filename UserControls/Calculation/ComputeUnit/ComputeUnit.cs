using System;

namespace Hexagon.Software.NCGage.ExpressionCalculator
{
  public abstract class ComputeUnit : IComputeUnit
  {
    public ComputeUnit(String strExpression)
    {
      this.Expression = NumericCalculator.Normalize(strExpression);
    }
    protected double GetRadians(double value, AngleUnits angleUnit)
    {
      return angleUnit == AngleUnits.Radians ? value : value * Math.PI / 180;
    }
    protected double GetReturnedAngleValue(double value, AngleUnits angleUnit)
    {
      return angleUnit == AngleUnits.Radians ? value : value * 180 / Math.PI;
    }

    public abstract double Compute(AngleUnits angleUnit);
    public String Expression { get; private set; }
  }
}

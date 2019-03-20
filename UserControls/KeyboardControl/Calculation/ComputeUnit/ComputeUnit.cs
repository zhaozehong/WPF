using System;

namespace Hexagon.Software.NCGage.ExpressionCalculator
{
  public abstract class ComputeUnit : IComputeUnit
  {
    public ComputeUnit(String strExpression)
    {
      this.Expression = NumericCalculator.Normalize(strExpression);
    }

    public abstract double Compute();
    public String Expression { get; private set; }
  }
}

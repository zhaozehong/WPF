using System;

namespace Hexagon.Software.NCGage.ExpressionCalculator
{
  public enum AngleUnits { Degree, Radians };
  public interface IComputeUnit: IExpression
  {
    Double Compute(AngleUnits angleUnit);
  }
}

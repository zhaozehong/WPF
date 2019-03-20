using System;

namespace Hexagon.Software.NCGage.ExpressionCalculator
{
  public interface IComputeUnit: IExpression
  {
    Double Compute();
  }
}

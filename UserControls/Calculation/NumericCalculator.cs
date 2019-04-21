using System;
using System.Linq;
using System.Text.RegularExpressions;
using Hexagon.Software.NCGage.HelperLib;


namespace Hexagon.Software.NCGage.ExpressionCalculator
{
  public class NumericCalculator
  {
    public static IComputeUnit Parse(String strNumericExpression)
    {
      var normalizedExpression = Normalize(strNumericExpression);
      if (Helpers.IsNumericValue(normalizedExpression))
        return new NumericUnit(normalizedExpression);

      String strExpression;
      SimpleFunctions simplefunction;
      if (TrySimpleParse(normalizedExpression, out strExpression, out simplefunction))
        return new SimpleUnit(strExpression, simplefunction);

      ComplexFunctions complexFunction;
      if (TryComplexParse(normalizedExpression, out strExpression, out complexFunction))
        return new ComplexUnit(strExpression, complexFunction);

      return null;
    }

    public static String Normalize(String strExpression)
    {
      if (String.IsNullOrWhiteSpace(strExpression))
        return "0";

      var expression = CorrectPrefixPlusMinus(strExpression);
      while (Regex.IsMatch(expression, @"^[([{]") && Regex.IsMatch(expression, @"[)\]}]$") && Helpers.GetSubExpression(expression) == expression)
      {
        expression = CorrectPrefixPlusMinus(expression.Substring(1, expression.Length - 2)); // remove unnecessary bounded "()"
      }

      if (Helpers.IsNumericValue(expression))
        return expression;
      if (expression[0] != '+' && expression[0] != '-')
        return expression;

      var matchs = Regex.Matches(expression, @"[\+\-*/%\,]").OfType<Match>().ToList(); // '+', '-', '*', '/', '%' , ','
      if (matchs.Count >= 2)
      {
        var temp = expression.Substring(0, matchs[1].Index);
        if (Helpers.IsNumericValue(temp)) // if expression starts with '+' or '-', add "()" to it.
        {
          expression = expression.Insert(matchs[1].Index, ")");
          expression = expression.Insert(0, "(");
          return expression;
        }
      }

      return "0" + expression;
    }

    private static Boolean TrySimpleParse(String normalizedExpression, out String strExpression, out SimpleFunctions function)
    {
      function = SimpleFunctions.None;
      strExpression = String.Copy(normalizedExpression);
      if (Regex.IsMatch(strExpression, @"^[0-9]"))
        return true;

      var subExpression = Helpers.GetSubExpression(strExpression);
      if (subExpression != strExpression)
        return true;

      var match = Regex.Match(strExpression, @"[([{]");
      if (!match.Success)
        return false;

      if (match.Index == 0)
        return true;

      var expression1 = strExpression.Substring(0, match.Index).ToUpper();
      var functionName = Enum.GetNames(typeof(SimpleFunctions)).FirstOrDefault(p => p.ToUpper() == expression1);

      if (functionName == null)
        return false;

      function = (SimpleFunctions)Enum.Parse(typeof(SimpleFunctions), functionName);
      strExpression = strExpression.Substring(functionName.Length + 1, strExpression.Length - functionName.Length - 2);
      return true;
    }
    private static Boolean TryComplexParse(String normalizedExpression, out String strExpression, out ComplexFunctions complexFunction)
    {
      complexFunction = ComplexFunctions.None;
      strExpression = String.Copy(normalizedExpression);
      if (Regex.IsMatch(strExpression, @"^[0-9]"))
        return false;

      var subExpression = Helpers.GetSubExpression(strExpression);
      if (subExpression != strExpression)
        return false;

      var match = Regex.Match(strExpression, @"[([{]");
      if (!match.Success)
        return false;

      var expression1 = strExpression.Substring(0, match.Index).ToUpper();
      var functionName = Enum.GetNames(typeof(ComplexFunctions)).FirstOrDefault(p => p.ToUpper() == expression1);
      if (functionName == null)
        return false;

      complexFunction = (ComplexFunctions)Enum.Parse(typeof(ComplexFunctions), functionName);
      strExpression = strExpression.Substring(functionName.Length + 1, strExpression.Length - functionName.Length - 2);
      return true;
    }
    private static String CorrectPrefixPlusMinus(String value)
    {
      var retValue = Regex.Replace(value, " ", "").TrimStart('+'); // remove empty char & remove prefix '+'
      if (retValue.StartsWith("-"))
      {
        retValue = retValue.TrimStart('-', '+').Insert(0, "-"); // remove extra '-' & '+', and finally add a prefix '-' to the retValue
      }
      return retValue;
    }
  }
}

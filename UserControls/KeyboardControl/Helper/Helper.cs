using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Linq;
using System.Windows.Input;
using System.Text.RegularExpressions;

namespace Hexagon.Software.NCGage.HelperLib
{
  public static class Helpers
  {
    public static Double MIN_VALUE = 1e-8;
    public static Boolean IsNullOrZero(Double? value)
    {
      return value == null || Double.IsNaN(value.Value) || Math.Abs(value.Value) < MIN_VALUE;
    }
    public static FormattedText MeasureText(TextBox textBox, double fontSize)
    {
      return new FormattedText(textBox.Text, System.Threading.Thread.CurrentThread.CurrentUICulture, FlowDirection.LeftToRight,
        new Typeface(textBox.FontFamily, textBox.FontStyle, textBox.FontWeight, textBox.FontStretch),
        fontSize, textBox.Foreground);
    }

    public static double CalculateMaxWidthFontSize(TextBox textBox)
    {
      var boxWidth = Double.IsNaN(textBox.Width) ? textBox.ActualWidth : textBox.Width;
      if (double.IsNaN(boxWidth))
        return textBox.FontSize;

      double endSize = textBox.FontSize;
      if (MeasureText(textBox, endSize).WidthIncludingTrailingWhitespace < boxWidth)
        return endSize;

      double startSize = 0, space = 1;
      while (endSize > 0)
      {
        var middleSize = (endSize + startSize) / 2;
        var textWidth = MeasureText(textBox, middleSize).WidthIncludingTrailingWhitespace;
        if (IsNullOrZero(textWidth - boxWidth))
        {
          return middleSize;
        }
        else if (middleSize < boxWidth)
        {
          if (MeasureText(textBox, middleSize + space).WidthIncludingTrailingWhitespace > boxWidth)
            return middleSize;
          startSize = middleSize + space;
        }
        else
        {
          endSize = middleSize - space;
        }
      }
      return endSize;
    }

    public static String ListToValue<T>(List<T> sourceList, String separator = "")
    {
      if (sourceList == null || !sourceList.Any())
        return String.Empty;

      var sb = new StringBuilder();
      if (!String.IsNullOrEmpty(separator))
      {
        sourceList.ForEach(p => sb.Append(String.Format("{0}{1}", p.ToString(), separator)));
        sb.Remove(sb.Length - 1, 1);
      }
      else
      {
        sourceList.ForEach(p => sb.Append(p.ToString()));
      }
      return sb.ToString();
    }

    public static void RaiseTextInputEvent(UIElement element, string text)
    {
      if (element == null)
        return;

      InputManager inputManager = InputManager.Current;
      InputDevice inputDevice = inputManager.PrimaryKeyboardDevice;
      TextComposition composition = new TextComposition(inputManager, element, text);
      TextCompositionEventArgs args = new TextCompositionEventArgs(inputDevice, composition);
      args.RoutedEvent = UIElement.PreviewTextInputEvent;
      element.RaiseEvent(args);
      args.RoutedEvent = UIElement.TextInputEvent;
      element.RaiseEvent(args);
    }

    public static int GetCount(String strSource, String pattern)
    {
      return String.IsNullOrWhiteSpace(strSource) ? 0 : Regex.Matches(strSource, pattern).OfType<Match>().Count();
    }
    public static Boolean IsNumericValue(String strValue)
    {
      return Regex.IsMatch(strValue, @"^-?\d+\.?\d*$");
    }
    public static String GetSubExpression(String strExpression, bool checkCount = true)
    {
      if (String.IsNullOrWhiteSpace(strExpression))
        return null;

      var leftIndexs = Regex.Matches(strExpression, @"[([{]").OfType<Match>().Select(m => m.Index).ToList();
      var rightIndexs = Regex.Matches(strExpression, @"[)\]}]").OfType<Match>().Select(m => m.Index).ToList();
      if (checkCount && leftIndexs.Count != rightIndexs.Count)
        return null;

      var stopIndexs = rightIndexs.Where((r, index) => leftIndexs.Count(l => l < r) == (index + 1));
      return stopIndexs.Any() ? strExpression.Substring(0, stopIndexs.FirstOrDefault() + 1) : strExpression;
    }
    public static String GetLastSubExpression(String strExpression)
    {
      var index = GetLastSubExpressionIndex(strExpression);
      return index != -1 ? strExpression.Substring(index) : null;
    }
    public static Int32 GetLastSubExpressionIndex(String strExpression)
    {
      if (String.IsNullOrWhiteSpace(strExpression))
        return -1;

      var leftIndexs = Regex.Matches(strExpression, @"[([{]").OfType<Match>().Select(m => m.Index).ToList();
      var rightIndexs = Regex.Matches(strExpression, @"[)\]}]").OfType<Match>().Select(m => m.Index).ToList();
      if (!leftIndexs.Any() || !rightIndexs.Any())
        return -1;

      var iEnd = rightIndexs.LastOrDefault();
      var stopIndexs = leftIndexs.Where((i) => rightIndexs.Count(r => r >= i && r <= iEnd) == leftIndexs.Count(l => l >= i && l <= iEnd));
      return stopIndexs.Any() ? stopIndexs.FirstOrDefault() : -1;
    }
    public static String GetValueString(Nullable<Double> value, Int32 decimals = 4)
    {
      if (value == null || Double.IsNaN(value.Value))
        return String.Empty;
      return String.Format("{0:F" + decimals.ToString() + "}", value);
    }
  }
}

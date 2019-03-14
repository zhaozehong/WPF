using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Linq;

namespace WPF.Helper
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
  }
}

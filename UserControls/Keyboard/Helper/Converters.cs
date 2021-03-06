﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Linq;

namespace Hexagon.Software.NCGage.HelperLib
{
  public sealed class EnumToVisibilityConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value == null || parameter == null)
        return Visibility.Visible;
      if (object.ReferenceEquals(value, parameter) || object.Equals(value, parameter))
        return Visibility.Visible;

      if (!value.GetType().IsEnum)
        throw new NotSupportedException();

      if (!Enum.IsDefined(value.GetType(), parameter.ToString()))
        return Visibility.Collapsed;

      return (value.ToString() == Enum.Parse(value.GetType(), parameter.ToString()).ToString()) ? Visibility.Visible : Visibility.Collapsed;
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotSupportedException();
    }
  }
  public sealed class BooleanToVisibilityReverseConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      try
      {
        if (value == null || !(value is bool))
          return Visibility.Visible;
        return ((bool)value == false) ? Visibility.Visible : Visibility.Collapsed;
      }
      catch (Exception)
      {
        return Visibility.Collapsed;
      }
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      try
      {
        Visibility vvalue = (Visibility)value;
        return (vvalue != Visibility.Visible);
      }
      catch (Exception)
      {
        return false;
      }
    }
  }
  public sealed class BooleanReverseConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      try
      {
        if (value == null && !(value is bool))
          return false;

        bool bvalue = (bool)value;
        bool ret = !bvalue;
        return ret;
      }
      catch (Exception)
      {
        return false;
      }
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      try
      {
        bool bvalue = (bool)value;
        return !bvalue;
      }
      catch (Exception)
      {
        return false;
      }
    }
  }
  public sealed class DoubleToThicknessConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      try
      {
        return new Thickness((value != null && (value is double)) ? (double)value : 0);
      }
      catch (Exception)
      {
        return null;
      }
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      try
      {
        if (value == null || !(value is Thickness))
          return 0;
        var thickness = (Thickness)value;
        var sd = new double[] { thickness.Left, thickness.Right, thickness.Top, thickness.Bottom };
        return sd.Average();
      }
      catch (Exception)
      {
        return false;
      }
    }
  }
  public sealed class IsNumericConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      try
      {
        if (value == null)
          return false;
        return Helpers.IsNumericValue(value.ToString());
      }
      catch (Exception)
      {
        return null;
      }
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      return 0.0;
    }
  }
  public sealed class DoubleToParamDoubleConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      try
      {
        if (value == null || !(value is double))
          return Double.NaN;
        if (parameter == null || !Helpers.IsNumericValue(parameter.ToString()))
          return value;
        return (double)value * double.Parse(parameter.ToString());
      }
      catch (Exception)
      {
        return Double.NaN;
      }
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      try
      {
        if (value == null || !(value is double))
          return Double.NaN;
        if (parameter == null || !Helpers.IsNumericValue(parameter.ToString()))
          return value;
        return (double)value / double.Parse(parameter.ToString());
      }
      catch (Exception)
      {
        return Double.NaN;
      }
    }
  }
  public sealed class ObjectIsTypeToVisibilityConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      string typeName = string.Format("{0}", parameter);
      return (value != null && value.GetType().Name == typeName) ? Visibility.Visible : Visibility.Collapsed;
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotSupportedException();
    }
  }
}

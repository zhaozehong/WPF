using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Helper
{
  public class MvvmBehaviors
  {
    public static String GetLoadedMethodName(DependencyObject obj)
    {
      return obj.GetValue(LoadedMethodNameProperty) as String;
    }
    public static void SetLoadedMethodName(DependencyObject obj, String value)
    {
      obj.SetValue(LoadedMethodNameProperty, value);
    }
    public static readonly DependencyProperty LoadedMethodNameProperty =
      DependencyProperty.RegisterAttached("LoadedMethodName", typeof(string), typeof(MvvmBehaviors),
        new PropertyMetadata(null, OnLoadedMethodNameChanged));

    private static void OnLoadedMethodNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var fe = d as FrameworkElement;
      if (fe == null || e.NewValue == null)
        return;

      fe.Loaded += (s, e2) =>
      {
        var viewModel = fe.DataContext;
        if (viewModel == null)
          return;

        var methodInfo = viewModel.GetType().GetMethod(e.NewValue.ToString());
        if (methodInfo == null)
          return;

        methodInfo.Invoke(viewModel, null);
      };
    }
  }
}

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using WPF.Helper;

namespace WPF.UserControls
{
  public partial class NumberKeyboardControl : KeyboardControl
  {
    public NumberKeyboardControl()
    {
      InitializeComponent();
    }

    private void btnSwitch_Click(object sender, RoutedEventArgs e)
    {
      this.FireSwitchedEvent(sender, e);
    }
    private void btnClose_Click(object sender, RoutedEventArgs e)
    {
      this.FireClosedEvent(sender, e);
    }

    private void OnButtonSizeChanged()
    {
      this.DisplayScreenWidth = this.ButtonSize * 4 + 16;
    }

    #region Dependency Properties
    public Double ButtonSize
    {
      get { return (Double)GetValue(ButtonSizeProperty); }
      set { SetValue(ButtonSizeProperty, value); }
    }

    public static readonly DependencyProperty ButtonSizeProperty =
        DependencyProperty.Register("ButtonSize", typeof(Double), typeof(NumberKeyboardControl), new PropertyMetadata(100.0, new PropertyChangedCallback(OnButtonSizeChanged)));

    private static void OnButtonSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var control = d as NumberKeyboardControl;
      if (control != null)
        control.OnButtonSizeChanged();
    }

    #endregion
  }
  public class NumberKeyboardControlViewModel : KeyboardViewModelBase { }
}

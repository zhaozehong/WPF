using System;
using System.Windows;

namespace WPF.UserControls
{
  public partial class CalculatorKeyboardControl : KeyboardControl
  {
    public CalculatorKeyboardControl()
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
      this.DisplayScreenWidth = this.ButtonSize * 9 + 16;
    }

    #region Dependency Properties
    public Double ButtonSize
    {
      get { return (Double)GetValue(ButtonSizeProperty); }
      set { SetValue(ButtonSizeProperty, value); }
    }

    public static readonly DependencyProperty ButtonSizeProperty =
        DependencyProperty.Register("ButtonSize", typeof(Double), typeof(CalculatorKeyboardControl), new PropertyMetadata(100.0, new PropertyChangedCallback(OnButtonSizeChanged)));

    private static void OnButtonSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var control = d as CalculatorKeyboardControl;
      if (control != null)
        control.OnButtonSizeChanged();
    }

    #endregion
  }
  public class CalculatorKeyboardControlViewModel : KeyboardViewModelBase { }
}

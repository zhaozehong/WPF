using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Hexagon.Software.NCGage.UserControls
{
  public class KeyboardPopup : PopupEx
  {
    public KeyboardPopup()
    {
      var keyboard = new CalculatorKeyboard();
      keyboard.SetBinding(CalculatorKeyboard.InputTargetProperty, new Binding("InputTarget") { Source = this });
      keyboard.SetBinding(CalculatorKeyboard.StartupKeyboardTypeProperty, new Binding("StartupKeyboardType") { Source = this });
      keyboard.SetBinding(CalculatorKeyboard.ButtonSizeProperty, new Binding("ButtonSize") { Source = this });
      keyboard.SetBinding(CalculatorKeyboard.ButtonMarginProperty, new Binding("ButtonMargin") { Source = this });
      keyboard.SetBinding(CalculatorKeyboard.ResetOnCalculationProperty, new Binding("ResetOnCalculation") { Source = this });
      keyboard.Closed += (s, e) => this.IsOpen = false;
      this.Child = keyboard;

      this.SetBinding(PopupEx.IsPinProperty, new Binding("IsPin") { Source = keyboard, Mode = BindingMode.TwoWay });
    }

    private void OnInputTargetChanged(TextBox oldTarget, TextBox newTarget)
    {
      if (KeyboardManager.Equals(this)) // only one instance
      {
        if (newTarget != null)
        {
          newTarget.GotFocus -= InputTarge_GotFocus;
          newTarget.LostFocus -= InputTarge_LostFocus;

          newTarget.GotFocus += InputTarge_GotFocus;
          newTarget.LostFocus += InputTarge_LostFocus;
        }
      }
      else
      {
        if (oldTarget != null)
        {
          oldTarget.GotFocus -= InputTarge_GotFocus;
          oldTarget.LostFocus -= InputTarge_LostFocus;
        }
        if (newTarget != null)
        {
          newTarget.GotFocus += InputTarge_GotFocus;
          newTarget.LostFocus += InputTarge_LostFocus;
        }
      }
    }
    private void InputTarge_GotFocus(object sender, RoutedEventArgs e)
    {
      if (KeyboardManager.Equals(this))
        KeyboardManager.GetKeyboard(sender as DependencyObject).IsOpen = true;
      else
        this.IsOpen = true;
    }
    private void InputTarge_LostFocus(object sender, RoutedEventArgs e)
    {
      this.IsOpen = false;
    }

    #region Dependency Properties
    public TextBox InputTarget
    {
      get { return (TextBox)GetValue(InputTargetProperty); }
      set { SetValue(InputTargetProperty, value); }
    }
    public static readonly DependencyProperty InputTargetProperty = DependencyProperty.Register("InputTarget", typeof(TextBox), typeof(KeyboardPopup), new PropertyMetadata(null, new PropertyChangedCallback(OnInputTargetChanged)));
    private static void OnInputTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var popup = d as KeyboardPopup;
      if (popup != null)
        popup.OnInputTargetChanged(e.OldValue as TextBox, e.NewValue as TextBox);
    }

    public KeyboardTypes StartupKeyboardType
    {
      get { return (KeyboardTypes)GetValue(StartupKeyboardTypeProperty); }
      set { SetValue(StartupKeyboardTypeProperty, value); }
    }
    public static readonly DependencyProperty StartupKeyboardTypeProperty = DependencyProperty.Register("StartupKeyboardType", typeof(KeyboardTypes), typeof(KeyboardPopup), new PropertyMetadata(KeyboardTypes.Number));

    public double ButtonSize
    {
      get { return (Double)GetValue(ButtonSizeProperty); }
      set { SetValue(ButtonSizeProperty, value); }
    }
    public static readonly DependencyProperty ButtonSizeProperty = DependencyProperty.Register("ButtonSize", typeof(double), typeof(KeyboardPopup), new PropertyMetadata(50.0));

    public double ButtonMargin
    {
      get { return (double)GetValue(ButtonMarginProperty); }
      set { SetValue(ButtonMarginProperty, value); }
    }
    public static readonly DependencyProperty ButtonMarginProperty = DependencyProperty.Register("ButtonMargin", typeof(double), typeof(KeyboardPopup), new PropertyMetadata(1.0));

    public bool ResetOnCalculation
    {
      get { return (Boolean)GetValue(ResetOnCalculationProperty); }
      set { SetValue(ResetOnCalculationProperty, value); }
    }
    public static readonly DependencyProperty ResetOnCalculationProperty = DependencyProperty.Register("ResetOnCalculation", typeof(bool), typeof(KeyboardPopup), new PropertyMetadata(true));

    #endregion
  }
}

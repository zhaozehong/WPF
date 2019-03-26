using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Hexagon.Software.NCGage.UserControls
{
  public abstract class KeyboardPopup : PopupEx
  {
    public KeyboardPopup()
    {
      if(Keyboard != null)
      {
        Keyboard.SetBinding(KeyboardControl.InputTargetProperty, new Binding("InputTarget") { Source = this });
        Keyboard.SetBinding(KeyboardControl.StartupKeyboardTypeProperty, new Binding("StartupKeyboardType") { Source = this });
        Keyboard.SetBinding(KeyboardControl.ButtonSizeProperty, new Binding("ButtonSize") { Source = this });
        Keyboard.SetBinding(KeyboardControl.ButtonMarginProperty, new Binding("ButtonMargin") { Source = this });
        Keyboard.Closed += (s, e) => this.IsOpen = false;

        this.Child = Keyboard;

        this.SetBinding(PopupEx.IsPinProperty, new Binding("IsPin") { Source = Keyboard, Mode = BindingMode.TwoWay });
      }
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

    protected abstract KeyboardControl Keyboard { get; }

    #region Dependency Properties
    public TextBox InputTarget
    {
      get { return (TextBox)GetValue(InputTargetProperty); }
      set { SetValue(InputTargetProperty, value); }
    }
    public static readonly DependencyProperty InputTargetProperty = DependencyProperty.Register("InputTarget", typeof(TextBox), typeof(CalculatorKeyboardPopup), new PropertyMetadata(null, new PropertyChangedCallback(OnInputTargetChanged)));
    private static void OnInputTargetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var popup = d as CalculatorKeyboardPopup;
      if (popup != null)
        popup.OnInputTargetChanged(e.OldValue as TextBox, e.NewValue as TextBox);
    }

    public KeyboardTypes StartupKeyboardType
    {
      get { return (KeyboardTypes)GetValue(StartupKeyboardTypeProperty); }
      set { SetValue(StartupKeyboardTypeProperty, value); }
    }
    public static readonly DependencyProperty StartupKeyboardTypeProperty = DependencyProperty.Register("StartupKeyboardType", typeof(KeyboardTypes), typeof(CalculatorKeyboardPopup), new PropertyMetadata(KeyboardTypes.Number));

    public double ButtonSize
    {
      get { return (Double)GetValue(ButtonSizeProperty); }
      set { SetValue(ButtonSizeProperty, value); }
    }
    public static readonly DependencyProperty ButtonSizeProperty = DependencyProperty.Register("ButtonSize", typeof(double), typeof(CalculatorKeyboardPopup), new PropertyMetadata(50.0));

    public double ButtonMargin
    {
      get { return (double)GetValue(ButtonMarginProperty); }
      set { SetValue(ButtonMarginProperty, value); }
    }
    public static readonly DependencyProperty ButtonMarginProperty = DependencyProperty.Register("ButtonMargin", typeof(double), typeof(CalculatorKeyboardPopup), new PropertyMetadata(1.0));

    #endregion
  }
}

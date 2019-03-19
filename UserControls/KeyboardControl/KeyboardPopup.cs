using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using WPF.Helper;

namespace WPF.UserControls
{
  public class KeyboardPopup : PopupEx
  {
    public KeyboardPopup()
    {
      var keyboard = new CalculatorKeyboard();
      keyboard.SetBinding(CalculatorKeyboard.ButtonSizeProperty, new Binding("ButtonSize") { Source = this });
      keyboard.SetBinding(CalculatorKeyboard.TargetElementProperty, new Binding("TargetElement") { Source = this });
      keyboard.Closed += Keyboard_Closed;

      this.Child = keyboard;

      this.SetBinding(KeyboardPopup.AutoUpdatePositionProperty, new Binding("IsPin") { Source = keyboard, Converter = new BooleanReverseConverter() });
    }

    private void OnInputTargetChanged(TextBox oldTarget, TextBox newTarget)
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
    private void InputTarge_GotFocus(object sender, RoutedEventArgs e)
    {
      this.IsOpen = true;
    }
    private void InputTarge_LostFocus(object sender, RoutedEventArgs e)
    {
      this.IsOpen = false;
    }
    private void Keyboard_Closed(object sender, EventArgs e)
    {
      this.IsOpen = false;
    }

    #region Dependency Properties
    public double ButtonSize
    {
      get { return (Double)GetValue(ButtonSizeProperty); }
      set { SetValue(ButtonSizeProperty, value); }
    }
    public static readonly DependencyProperty ButtonSizeProperty = DependencyProperty.Register("ButtonSize", typeof(double), typeof(KeyboardPopup), new PropertyMetadata(50.0));

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

    #endregion
  }
}

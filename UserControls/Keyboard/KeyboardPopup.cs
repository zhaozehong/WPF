using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Hexagon.Software.NCGage.HelperLib;

namespace Hexagon.Software.NCGage.UserControls
{
  public class KeyboardPopup : PopupEx
  {
    public KeyboardPopup()
    {
      var keyboard = new KeyboardControl();
      keyboard.SetBinding(KeyboardControlBase.InputTargetProperty, new Binding("InputTarget") { Source = this });
      keyboard.SetBinding(KeyboardControlBase.StartupKeyboardTypeProperty, new Binding("StartupKeyboardType") { Source = this });
      keyboard.SetBinding(KeyboardControlBase.ButtonSizeProperty, new Binding("ButtonSize") { Source = this });
      keyboard.SetBinding(KeyboardControlBase.ButtonMarginProperty, new Binding("ButtonMargin") { Source = this });
      keyboard.SetBinding(CalculatorKeyboardControl.ResetOnCalculationProperty, new Binding("ResetOnCalculation") { Source = this });
      keyboard.Closed += (s, e) => this.IsOpen = false;

      //ZEHONG: use a Viewbox to make keyboard become big enough to fit inside the available space
      this.Child = new Viewbox() { Child = keyboard };

      this.SetBinding(PopupEx.IsPinProperty, new Binding("IsPin") { Source = keyboard, Mode = BindingMode.TwoWay });
    }

    protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
    {
      base.OnMouseRightButtonDown(e);
      if (!Keyboard.IsKeyDown(Key.LeftCtrl) && !Keyboard.Modifiers.HasFlag(ModifierKeys.Alt))
        return;

      if (this.StartupKeyboardType == KeyboardTypes.Full)
        this.StartupKeyboardType = KeyboardTypes.Number;
      else if (this.StartupKeyboardType == KeyboardTypes.Number)
        this.StartupKeyboardType = KeyboardTypes.Calculator;
      else
        this.StartupKeyboardType = KeyboardTypes.Full;
    }

    private void OnInputTargetChanged(TextBox oldTarget, TextBox newTarget)
    {
      if (KeyboardManager.Equals(this)) //ZEHONG: never remove event handler from oldTarget if you're using KeyboardManager, or it will never receive GotFocus&LostFocus except for the last TextBox.
      {
        if (newTarget != null)
        {
          newTarget.GotFocus -= InputTarge_GotFocus;
          newTarget.LostFocus -= InputTarge_LostFocus;

          newTarget.GotFocus += InputTarge_GotFocus;
          newTarget.LostFocus += InputTarge_LostFocus;
        }
      }
      else // if you are using Keyboard popup separately, you need to remove the old handler for each popup
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
      try
      {
        if (KeyboardManager.Equals(this))
          KeyboardManager.GetPopup(sender as DependencyObject).IsOpen = true;
        else
          this.IsOpen = true;
      }
      catch (Exception) { }
    }
    private void InputTarge_LostFocus(object sender, RoutedEventArgs e)
    {
      if (this.IsOpen)
        this.IsOpen = false;
    }

    #region Dependency Properties
    public TextBox InputTarget
    {
      get { return (TextBox)GetValue(InputTargetProperty); }
      set { SetValue(InputTargetProperty, value); }
    }
    public static readonly DependencyProperty InputTargetProperty = DependencyProperty.Register("InputTarget", typeof(TextBox), typeof(KeyboardPopup), new PropertyMetadata(null, OnInputTargetChanged));
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

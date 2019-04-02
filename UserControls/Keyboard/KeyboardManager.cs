using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Hexagon.Software.NCGage.UserControls
{
  public class KeyboardManager
  {
    public static bool Equals(KeyboardPopup popupObj)
    {
      return KeyboardPopupObj.Equals(popupObj);
    }
    public static KeyboardPopup GetPopup(DependencyObject obj)
    {
      if (!GetEnabled(obj))
        return null;

      if (!obj.Equals(KeyboardPopupObj.PlacementTarget))
      {
        // apply user defined properties to update the keyboardPopup object
        KeyboardPopupObj.StartupKeyboardType = GetStartupKeyboardType(obj);
        KeyboardPopupObj.Topmost = GetTopmost(obj);
        KeyboardPopupObj.ButtonSize = GetButtonSize(obj);
        KeyboardPopupObj.ButtonMargin = GetButtonMargin(obj);
        KeyboardPopupObj.InputTarget = obj as TextBox;
        KeyboardPopupObj.PlacementTarget = KeyboardPopupObj.InputTarget;
        KeyboardPopupObj.SharePosition = GetSharePosition(obj);
        KeyboardPopupObj.Placement = GetPlacement(obj);
        KeyboardPopupObj.ResetOnCalculation = GetResetOnCalculation(obj);
      }
      return KeyboardPopupObj;
    }

    #region Attached Properties
    public static bool GetEnabled(DependencyObject obj)
    {
      return (bool)obj.GetValue(EnabledProperty);
    }
    public static void SetEnabled(DependencyObject obj, bool value)
    {
      obj.SetValue(EnabledProperty, value);
    }
    public static readonly DependencyProperty EnabledProperty = DependencyProperty.RegisterAttached("Enabled", typeof(bool), typeof(KeyboardManager), new PropertyMetadata(false, OnEnableChanged));
    private static void OnEnableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      if ((bool)e.NewValue)
        KeyboardPopupObj.InputTarget = d as TextBox; //ZEHONG: to subscribe GotFocus & LostFocus event to Open/Close keyboard popup
    }

    public static KeyboardTypes GetStartupKeyboardType(DependencyObject obj)
    {
      return (KeyboardTypes)obj.GetValue(StartupKeyboardTypeProperty);
    }
    public static void SetStartupKeyboardType(DependencyObject obj, KeyboardTypes value)
    {
      obj.SetValue(StartupKeyboardTypeProperty, value);
    }
    public static readonly DependencyProperty StartupKeyboardTypeProperty = DependencyProperty.RegisterAttached("StartupKeyboardType", typeof(KeyboardTypes), typeof(KeyboardManager), new PropertyMetadata(KeyboardTypes.Number));

    public static bool GetTopmost(DependencyObject obj)
    {
      return (bool)obj.GetValue(TopmostProperty);
    }
    public static void SetTopmost(DependencyObject obj, bool value)
    {
      obj.SetValue(TopmostProperty, value);
    }
    public static DependencyProperty TopmostProperty = DependencyProperty.RegisterAttached("Topmost", typeof(bool), typeof(KeyboardManager), new PropertyMetadata(false));

    public static double GetButtonSize(DependencyObject obj)
    {
      return (double)obj.GetValue(ButtonSizeProperty);
    }
    public static void SetButtonSize(DependencyObject obj, double value)
    {
      obj.SetValue(ButtonSizeProperty, value);
    }
    public static readonly DependencyProperty ButtonSizeProperty = DependencyProperty.RegisterAttached("ButtonSize", typeof(double), typeof(KeyboardManager), new PropertyMetadata(50.0));

    public static double GetButtonMargin(DependencyObject obj)
    {
      return (double)obj.GetValue(ButtonMarginProperty);
    }
    public static void SetButtonMargin(DependencyObject obj, double value)
    {
      obj.SetValue(ButtonMarginProperty, value);
    }
    public static readonly DependencyProperty ButtonMarginProperty = DependencyProperty.RegisterAttached("ButtonMargin", typeof(double), typeof(KeyboardManager), new PropertyMetadata(1.0));

    public static bool GetResetOnCalculation(DependencyObject obj)
    {
      return (bool)obj.GetValue(ResetOnCalculationProperty);
    }
    public static void SetResetOnCalculation(DependencyObject obj, bool value)
    {
      obj.SetValue(ResetOnCalculationProperty, value);
    }
    public static readonly DependencyProperty ResetOnCalculationProperty = DependencyProperty.RegisterAttached("ResetOnCalculation", typeof(bool), typeof(KeyboardManager), new PropertyMetadata(true));

    public static bool GetSharePosition(DependencyObject obj)
    {
      return (bool)obj.GetValue(SharePositionProperty);
    }
    public static void SetSharePosition(DependencyObject obj, bool value)
    {
      obj.SetValue(SharePositionProperty, value);
    }
    public static readonly DependencyProperty SharePositionProperty = DependencyProperty.RegisterAttached("SharePosition", typeof(bool), typeof(KeyboardManager), new PropertyMetadata(true));

    public static PlacementMode GetPlacement(DependencyObject obj)
    {
      return (PlacementMode)obj.GetValue(PlacementProperty);
    }
    public static void SetPlacement(DependencyObject obj, PlacementMode value)
    {
      obj.SetValue(PlacementProperty, value);
    }
    public static readonly DependencyProperty PlacementProperty = DependencyProperty.RegisterAttached("Placement", typeof(PlacementMode), typeof(KeyboardManager), new PropertyMetadata(PlacementMode.Bottom));

    #endregion

    private readonly static KeyboardPopup KeyboardPopupObj = new KeyboardPopup();
  }
}

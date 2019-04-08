using System;
using System.Windows;
using System.Windows.Input;

namespace Hexagon.Software.NCGage.UserControls
{
  public class FullKeyboardButton : KeyboardButton
  {
    public bool SendKey()
    {
      var hasPressedCapsLock = false;
      try
      {
        if (this.Content == null && this.Key == Key.None)
          return false;

        if (Console.CapsLock)
        {
          FullKeyboardHelper.FireKeyPress(CapLockVirtualKey);
          hasPressedCapsLock = true;
        }

        if (this.Key != Key.None)
        {
          FullKeyboardHelper.FireKeyPress(Convert.ToByte(KeyInterop.VirtualKeyFromKey(this.Key)));
        }
        else if (this.Content.GetType() == typeof(String))
        {
          FullKeyAction action;
          if (FullKeyboardHelper.CharActionPairs.TryGetValue(this.Content.ToString(), out action))
          {
            if (action.IsShift) FullKeyboardHelper.FireKeyDown(ShiftVirtualKey);
            FullKeyboardHelper.FireKeyPress(Convert.ToByte(KeyInterop.VirtualKeyFromKey(action.CurrentKey)));
            if (action.IsShift) FullKeyboardHelper.FireKeyUp(ShiftVirtualKey);
          }
        }
        return true;
      }
      catch (Exception)
      {
        return false;
      }
      finally
      {
        if (hasPressedCapsLock)
          FullKeyboardHelper.FireKeyPress(CapLockVirtualKey);
      }
    }

    // provided for non-content(invisible) key
    public Key Key
    {
      get { return (Key)GetValue(KeyProperty); }
      set { SetValue(KeyProperty, value); }
    }
    public static readonly DependencyProperty KeyProperty = DependencyProperty.Register("Key", typeof(Key), typeof(FullKeyboardButton), new PropertyMetadata(Key.None));

    public string NormalText
    {
      get { return (string)GetValue(TextProperty); }
      set { SetValue(TextProperty, value); }
    }
    public static readonly DependencyProperty TextProperty = DependencyProperty.Register("NormalText", typeof(string), typeof(FullKeyboardButton), new PropertyMetadata(null));

    public string ShiftText
    {
      get { return (string)GetValue(ShiftTextProperty); }
      set { SetValue(ShiftTextProperty, value); }
    }
    public static readonly DependencyProperty ShiftTextProperty = DependencyProperty.Register("ShiftText", typeof(string), typeof(FullKeyboardButton), new PropertyMetadata(null));

    public string SymbolText
    {
      get { return (string)GetValue(SymbolTextProperty); }
      set { SetValue(SymbolTextProperty, value); }
    }
    public static readonly DependencyProperty SymbolTextProperty = DependencyProperty.Register("SymbolText", typeof(string), typeof(FullKeyboardButton), new PropertyMetadata(null));

    public string JapaneseText
    {
      get { return (string)GetValue(JapaneseTextProperty); }
      set { SetValue(JapaneseTextProperty, value); }
    }
    public static readonly DependencyProperty JapaneseTextProperty = DependencyProperty.Register("JapaneseText", typeof(string), typeof(FullKeyboardButton), new PropertyMetadata(null));


    private static byte CapLockVirtualKey = Convert.ToByte(KeyInterop.VirtualKeyFromKey(Key.CapsLock));
    private static byte ShiftVirtualKey = Convert.ToByte(KeyInterop.VirtualKeyFromKey(Key.LeftShift));
  }
}

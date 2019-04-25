using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;

namespace Hexagon.Software.NCGage.UserControls
{
  public class FullKeyboardButton : KeyboardButton
  {
    [StructLayout(LayoutKind.Explicit)]
    struct CharInputInfo
    {
      [FieldOffset(0)] public short Value;
      [FieldOffset(0)] public byte VirtualKeyCode;
      [FieldOffset(1)] public byte ControlKeys;
    }
    [DllImport("user32.dll")] static extern short VkKeyScan(char ch);

    public bool Fire()
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
          var strKeyContent = this.Content.ToString().Trim();
          if (string.IsNullOrWhiteSpace(strKeyContent))
            return false;

          char currentChar;
          if(!FullKeyboardHelper.CharKeyboardCharPairs.TryGetValue(strKeyContent, out currentChar))
            currentChar = strKeyContent[0];

          var helper = new CharInputInfo { Value = VkKeyScan(currentChar) };
          byte vkValue = helper.VirtualKeyCode;
          bool isShiftPressed = (helper.ControlKeys & 1) != 0;

          if (isShiftPressed) FullKeyboardHelper.FireKeyDown(ShiftVirtualKey);
          FullKeyboardHelper.FireKeyPress(vkValue);
          if (isShiftPressed) FullKeyboardHelper.FireKeyUp(ShiftVirtualKey);
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

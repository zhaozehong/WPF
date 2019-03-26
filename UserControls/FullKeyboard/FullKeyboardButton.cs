using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Hexagon.Software.NCGage.UserControls
{
  public class FullKeyboardButton : KeyboardButton
  {
    public bool SendKey(bool isShiftChecked)
    {
      if(isShiftChecked)
      {

      }
      KeyInterop.KeyFromVirtualKey(81); // q
      KeyInterop.VirtualKeyFromKey(Key.Q);
      return true;
    }

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

    public string JapaneseText
    {
      get { return (string)GetValue(JapaneseTextProperty); }
      set { SetValue(JapaneseTextProperty, value); }
    }
    public static readonly DependencyProperty JapaneseTextProperty = DependencyProperty.Register("JapaneseText", typeof(string), typeof(FullKeyboardButton), new PropertyMetadata(null));


  }
}

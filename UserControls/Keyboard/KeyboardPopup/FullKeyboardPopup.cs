using System;
using System.Windows;
using System.Windows.Data;

namespace Hexagon.Software.NCGage.UserControls
{
  public class FullKeyboardPopup : KeyboardPopup
  {
    protected KeyboardControl _keyboard;
    protected override KeyboardControl Keyboard
    {
      get
      {
        if (_keyboard == null)
          _keyboard = new FullKeyboardControl();
        return _keyboard;
      }
    }
  }
}

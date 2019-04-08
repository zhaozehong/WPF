using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace Hexagon.Software.NCGage.UserControls
{
  public class FullKeyAction
  {
    public bool IsShift { get; set; } = false;
    public Key CurrentKey { get; set; } = Key.None;
  }

  public static class FullKeyboardHelper
  {
    private static String LockObject = "LOCK STRING";
    private static Dictionary<string, FullKeyAction> _CharActionPairs = null;
    public static Dictionary<string, FullKeyAction> CharActionPairs
    {
      get
      {
        lock (LockObject)
        {
          if (_CharActionPairs == null)
          {
            _CharActionPairs = new Dictionary<string, FullKeyAction>(StringComparer.CurrentCulture);
            // letter
            for (int i = 65; i <= 90; i++)
            {
              var key = KeyInterop.KeyFromVirtualKey(i);
              // lower case
              _CharActionPairs.Add(key.ToString().ToLower(), new FullKeyAction() { CurrentKey = key });
              // upper case
              _CharActionPairs.Add(key.ToString().ToUpper(), new FullKeyAction() { CurrentKey = key, IsShift = true });
            }

            // digit (48~57 is D0~D9)
            for (int i = 96; i <= 105; i++)
            {
              _CharActionPairs.Add((i - 96).ToString(), new FullKeyAction() { CurrentKey = KeyInterop.KeyFromVirtualKey(i) });
            }

            // special definitions
            _CharActionPairs.Add("tab", new FullKeyAction() { CurrentKey = Key.Tab });
            _CharActionPairs.Add("DEL", new FullKeyAction() { CurrentKey = Key.Delete });

            // other chars
            _CharActionPairs.Add(")", new FullKeyAction() { CurrentKey = Key.D0, IsShift = true });
            _CharActionPairs.Add("!", new FullKeyAction() { CurrentKey = Key.D1, IsShift = true });
            _CharActionPairs.Add("@", new FullKeyAction() { CurrentKey = Key.D2, IsShift = true });
            _CharActionPairs.Add("#", new FullKeyAction() { CurrentKey = Key.D3, IsShift = true });
            _CharActionPairs.Add("$", new FullKeyAction() { CurrentKey = Key.D4, IsShift = true });
            _CharActionPairs.Add("%", new FullKeyAction() { CurrentKey = Key.D5, IsShift = true });
            _CharActionPairs.Add("^", new FullKeyAction() { CurrentKey = Key.D6, IsShift = true });
            _CharActionPairs.Add("&", new FullKeyAction() { CurrentKey = Key.D7, IsShift = true });
            _CharActionPairs.Add("*", new FullKeyAction() { CurrentKey = Key.D8, IsShift = true });
            _CharActionPairs.Add("(", new FullKeyAction() { CurrentKey = Key.D9, IsShift = true });

            _CharActionPairs.Add("`", new FullKeyAction() { CurrentKey = Key.Oem3 });
            _CharActionPairs.Add("~", new FullKeyAction() { CurrentKey = Key.Oem3, IsShift = true });

            _CharActionPairs.Add("-", new FullKeyAction() { CurrentKey = Key.OemMinus });
            _CharActionPairs.Add("_", new FullKeyAction() { CurrentKey = Key.OemMinus, IsShift = true });

            _CharActionPairs.Add("=", new FullKeyAction() { CurrentKey = Key.OemPlus });
            _CharActionPairs.Add("+", new FullKeyAction() { CurrentKey = Key.OemPlus, IsShift = true });

            _CharActionPairs.Add("[", new FullKeyAction() { CurrentKey = Key.Oem4 });
            _CharActionPairs.Add("{", new FullKeyAction() { CurrentKey = Key.Oem4, IsShift = true });

            _CharActionPairs.Add("]", new FullKeyAction() { CurrentKey = Key.Oem6 });
            _CharActionPairs.Add("}", new FullKeyAction() { CurrentKey = Key.Oem6, IsShift = true });

            _CharActionPairs.Add(@"\", new FullKeyAction() { CurrentKey = Key.Oem5 });
            _CharActionPairs.Add("|", new FullKeyAction() { CurrentKey = Key.Oem5, IsShift = true });

            _CharActionPairs.Add(";", new FullKeyAction() { CurrentKey = Key.Oem1 });
            _CharActionPairs.Add(":", new FullKeyAction() { CurrentKey = Key.Oem1, IsShift = true });
            
            _CharActionPairs.Add("'", new FullKeyAction() { CurrentKey = Key.Oem7 });
            _CharActionPairs.Add("\"", new FullKeyAction() { CurrentKey = Key.Oem7, IsShift = true });

            _CharActionPairs.Add(",", new FullKeyAction() { CurrentKey = Key.OemComma });
            _CharActionPairs.Add("<", new FullKeyAction() { CurrentKey = Key.OemComma, IsShift = true });

            _CharActionPairs.Add(".", new FullKeyAction() { CurrentKey = Key.OemPeriod });
            _CharActionPairs.Add(">", new FullKeyAction() { CurrentKey = Key.OemPeriod, IsShift = true });

            _CharActionPairs.Add("/", new FullKeyAction() { CurrentKey = Key.Oem2 });
            _CharActionPairs.Add("?", new FullKeyAction() { CurrentKey = Key.Oem2, IsShift = true });
          }
        }
        return _CharActionPairs;
      }
    }


    /// <summary> Simulate KeyPress & KeyDown & KeyUp </summary>
    public static void FireKeyPress(byte keyCode)
    {
      FireKeyDown(keyCode);
      FireKeyUp(keyCode);
    }
    public static void FireKeyDown(byte keyCode)
    {
      SendKeyboardEvent(keyCode, 0, 0, 0);
    }
    public static void FireKeyUp(byte keyCode)
    {
      SendKeyboardEvent(keyCode, 0, 2, 0);
    }


    [DllImport("user32.dll", EntryPoint = "keybd_event")]
    public static extern void SendKeyboardEvent(
    byte bVk, // virtual key
    byte bScan, // 0
    int dwFlags, // 0 - keydown，2 - keyup  
    int dwExtraInfo // 0  
    );

    [DllImport("user32.dll")]
    public static extern int GetFocus();
  }
}

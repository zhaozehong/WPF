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




    #region 模拟按键
    public static void Play()
    {
      keybd_event(179, 0, 0, 0);
      keybd_event(179, 0, 2, 0);
    }
    public static void Stop()
    {
      keybd_event(178, 0, 0, 0);
      keybd_event(178, 0, 2, 0);
    }

    public static void Last()
    {
      keybd_event(177, 0, 0, 0);
      keybd_event(177, 0, 2, 0);
    }
    public static void Next()
    {
      keybd_event(176, 0, 0, 0);
      keybd_event(176, 0, 2, 0);
    }
    #endregion

    /// <summary> 模拟按下指定建 Keys.ControlKey </summary>
    public static void OnKeyPress(byte keyCode)
    {
      OnKeyDown(keyCode);
      OnKeyUp(keyCode);
    }
    public static void OnKeyDown(byte keyCode)
    {
      keybd_event(keyCode, 0, 0, 0);
    }
    public static void OnKeyUp(byte keyCode)
    {
      keybd_event(keyCode, 0, 2, 0);
    }


    [DllImport("user32.dll", EntryPoint = "keybd_event")]
    public static extern void keybd_event(
    byte bVk, //虚拟键值  
    byte bScan,// 一般为0  
    int dwFlags, //这里是整数类型 0 为按下，2为释放  
    int dwExtraInfo //这里是整数类型 一般情况下设成为0  
    );

    [DllImport("user32.dll")]
    public static extern int GetFocus();
  }
}

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Input;

namespace Hexagon.Software.NCGage.UserControls
{
  public static class FullKeyboardHelper
  {
    private static string LockObject = "LOCK STRING";

    private static Dictionary<string, char> _CharKeyboardCharPairs = null;
    public static Dictionary<string, char> CharKeyboardCharPairs
    {
      get
      {
        lock (LockObject)
        {
          if (_CharKeyboardCharPairs == null)
          {
            _CharKeyboardCharPairs = new Dictionary<string, char>();

            _CharKeyboardCharPairs.Add("た", 'q');
            _CharKeyboardCharPairs.Add("て", 'w');
            _CharKeyboardCharPairs.Add("ぃ", 'e');
            _CharKeyboardCharPairs.Add("す", 'r');
            _CharKeyboardCharPairs.Add("か", 't');
            _CharKeyboardCharPairs.Add("ん", 'y');
            _CharKeyboardCharPairs.Add("な", 'u');
            _CharKeyboardCharPairs.Add("に", 'i');
            _CharKeyboardCharPairs.Add("ら", 'o');
            _CharKeyboardCharPairs.Add("せ", 'p');


            _CharKeyboardCharPairs.Add("ち", 'a');
            _CharKeyboardCharPairs.Add("と", 's');
            _CharKeyboardCharPairs.Add("し", 'd');
            _CharKeyboardCharPairs.Add("は", 'f');
            _CharKeyboardCharPairs.Add("き", 'g');
            _CharKeyboardCharPairs.Add("く", 'h');
            _CharKeyboardCharPairs.Add("ま", 'j');
            _CharKeyboardCharPairs.Add("の", 'k');
            _CharKeyboardCharPairs.Add("り", 'l');

            _CharKeyboardCharPairs.Add("つ", 'z');
            _CharKeyboardCharPairs.Add("さ", 'x');
            _CharKeyboardCharPairs.Add("そ", 'c');
            _CharKeyboardCharPairs.Add("ひ", 'v');
            _CharKeyboardCharPairs.Add("こ", 'b');
            _CharKeyboardCharPairs.Add("み", 'n');
            _CharKeyboardCharPairs.Add("も", 'm');

            // extra
            _CharKeyboardCharPairs.Add("ろ", '`');
            _CharKeyboardCharPairs.Add("ぬ", '1');
            _CharKeyboardCharPairs.Add("ふ", '2');
            _CharKeyboardCharPairs.Add("あ", '3');
            _CharKeyboardCharPairs.Add("う", '4');
            _CharKeyboardCharPairs.Add("え", '5');
            _CharKeyboardCharPairs.Add("お", '6');
            _CharKeyboardCharPairs.Add("や", '7');
            _CharKeyboardCharPairs.Add("ゆ", '8');
            _CharKeyboardCharPairs.Add("よ", '9');
            _CharKeyboardCharPairs.Add("わ", '0');
            _CharKeyboardCharPairs.Add("ほ", '-');
            _CharKeyboardCharPairs.Add("へ", '=');

            _CharKeyboardCharPairs.Add("゛", '[');
            _CharKeyboardCharPairs.Add("゜", ']');
            _CharKeyboardCharPairs.Add("む", '\\');

            
            _CharKeyboardCharPairs.Add("れ", ';');
            _CharKeyboardCharPairs.Add("け", '\'');

            _CharKeyboardCharPairs.Add("ね", ',');
            _CharKeyboardCharPairs.Add("る", '.');
            _CharKeyboardCharPairs.Add("め", '/');
          }
        }
        return _CharKeyboardCharPairs;
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

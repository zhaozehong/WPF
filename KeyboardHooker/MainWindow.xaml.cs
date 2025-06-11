using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace KeyboardHooker
{
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();

      this.Loaded += (s, e) =>
      {
        _hooker = new KeyboardHook();
        _hooker.Hook();
        _hooker.OnKeyDownEvent += kh_OnKeyDownEvent;
      };
      this.Closing += (s, e) => _hooker.Unhook();
    }

    void kh_OnKeyDownEvent(object sender, KeyEventArgs e)
    {
      if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
      {
        if (e.Key == Key.S) { this.Show(); } // Ctrl+S 显示窗口
        if (e.Key == Key.H) { this.Hide(); } // Ctrl+H 隐藏窗口
        if (e.Key == Key.C) { this.Close(); } // Ctrl+C 关闭窗口 
      }

      if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control && (Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt && e.Key == Key.A)
      {
        this.tbText.Text = "你发现了什么？"; // Ctrl+Alt+A
      }

      this.HandleKey(e.Key);
      this.tbText.Text = _sb.ToString();
    }

    private Boolean isCapsEnabled = false;
    private Boolean IsCtrlAltShiftKeys(Key key)
    {
      return key == Key.LeftCtrl || key == Key.RightCtrl || key == Key.LeftAlt || key == Key.RightAlt || key == Key.LeftShift || key == Key.RightShift;
    }
    private void HandleKey(Key key)
    {
      // virtual key: 8  --->  Back
      // virtual key: 9  --->  Tab
      // virtual key: 12  --->  CLEAR
      // virtual key: 13  --->  ENTER
      // virtual key: 16,17,18  --->  SHIFT,CTRL,ALT
      // virtual key: 20  --->  Caps Lock
      // virtual key: 32  --->  Space
      // virtual key: 35  --->  END
      // virtual key: 36  --->  HOME
      // virtual key: 37,39  --->  LEFT & RIGHT
      // virtual key: 38,40  --->  UP & DOWN
      // virtual key: 46  --->  DEL
      // virtual key: 48~57  --->  0~9
      // virtual key: 65~90  --->  A~Z
      // virtual key: 96~105  --->  0~9

      // virtual key: 106,107,109,111  --->  Multiply,Add,Substract,Divide
      // virtual key: 107,109  --->  ADD & SUBSTRACT
      // virtual key: 160, 161  --->  Left&Right Shift
      // virtual key: 162, 163  --->  Left&Right Ctrl
      // virtual key: 164, 164  --->  Left&Right Alt

      //var virtualKey = KeyInterop.VirtualKeyFromKey(key);
      switch (key)
      {
        case Key.LeftShift:
          break;
        case Key.Back:
          _sb.Remove(_sb.Length - 1, 1);
          break;
        case Key.Tab:
          _sb.Append("\t");
          break;
        case Key.Enter:
          _sb.Append(Environment.NewLine);
          break;
        case Key.Space:
          _sb.Append(" ");
          break;
        case Key.CapsLock:
          isCapsEnabled = !isCapsEnabled;
          break;
        case Key.D0:
          _sb.Append((Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.Shift ? "0" : ")");
          break;
        case Key.D1:
          _sb.Append((Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.Shift ? "1" : "!");
          break;
        case Key.D2:
          _sb.Append((Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.Shift ? "2" : "@");
          break;
        case Key.D3:
          _sb.Append((Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.Shift ? "3" : "#");
          break;
        case Key.D4:
          _sb.Append((Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.Shift ? "4" : "$");
          break;
        case Key.D5:
          _sb.Append((Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.Shift ? "5" : "%");
          break;
        case Key.D6:
          _sb.Append((Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.Shift ? "6" : "^");
          break;
        case Key.D7:
          _sb.Append((Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.Shift ? "7" : "&");
          break;
        case Key.D8:
          _sb.Append((Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.Shift ? "8" : "*");
          break;
        case Key.D9:
          _sb.Append((Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.Shift ? "9" : "(");
          break;
        case Key.NumPad0:
        case Key.NumPad1:
        case Key.NumPad2:
        case Key.NumPad3:
        case Key.NumPad4:
        case Key.NumPad5:
        case Key.NumPad6:
        case Key.NumPad7:
        case Key.NumPad8:
        case Key.NumPad9:
          _sb.Append(key.ToString().Last());
          break;
        case Key.A:
        case Key.B:
        case Key.C:
        case Key.D:
        case Key.E:
        case Key.F:
        case Key.G:
        case Key.H:
        case Key.I:
        case Key.J:
        case Key.K:
        case Key.L:
        case Key.M:
        case Key.N:
        case Key.O:
        case Key.P:
        case Key.Q:
        case Key.R:
        case Key.S:
        case Key.T:
        case Key.U:
        case Key.V:
        case Key.W:
        case Key.X:
        case Key.Y:
        case Key.Z:
          if ((!isCapsEnabled && (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift) || (isCapsEnabled && (Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.Shift))
            _sb.Append(key.ToString().ToUpper());
          else
            _sb.Append(key.ToString().ToLower());
          break;
        case Key.Add:
          _sb.Append("+");
          break;
        case Key.Subtract:
          _sb.Append("-");
          break;
        case Key.Multiply:
          _sb.Append("*");
          break;
        case Key.Divide:
          _sb.Append(@"/");
          break;
        case Key.OemPlus:
          _sb.Append((Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.Shift ? "=" : "+");
          break;
        case Key.OemMinus:
          _sb.Append((Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.Shift ? "-" : "_");
          break;
        case Key.Oem1:
          _sb.Append((Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.Shift ? ";" : ":");
          break;
        case Key.Oem2:
          _sb.Append((Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.Shift ? "/" : "?");
          break;
        case Key.Oem3:
          _sb.Append((Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.Shift ? "`" : "~");
          break;
        case Key.Oem4:
          _sb.Append((Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.Shift ? "[" : "{");
          break;
        case Key.Oem5:
          _sb.Append((Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.Shift ? "\\" : "|");
          break;
        case Key.Oem6:
          _sb.Append((Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.Shift ? "]" : "}");
          break;
        case Key.Oem7:
          _sb.Append((Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.Shift ? "'" : "\"");
          break;
        case Key.OemComma:
          _sb.Append((Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.Shift ? "," : "<");
          break;
        case Key.OemPeriod:
          _sb.Append((Keyboard.Modifiers & ModifierKeys.Shift) != ModifierKeys.Shift ? "." : ">");
          break;
        case Key.RightAlt:
          _sb.Clear();
          break;
        default:
          //_sb.Append((new KeyConverter()).ConvertToString(key));
          break;
      }
    }

    private readonly StringBuilder _sb = new StringBuilder();
    private KeyboardHook _hooker;
  }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Interop;

namespace KeyboardHooker
{
  public class KeyboardHook
  {
    int hHook;
    Win32Api.HookProc _handler;
    public event KeyEventHandler OnKeyDownEvent;
    public event KeyEventHandler OnKeyUpEvent;

    public void Hook()
    {
      _handler = new Win32Api.HookProc(OnHook);
      var cModule = Process.GetCurrentProcess().MainModule;
      var mh = Win32Api.GetModuleHandle(cModule.ModuleName);
      hHook = Win32Api.SetWindowsHookEx(Win32Api.WH_KEYBOARD_LL, _handler, mh, 0);
    }
    public void Unhook()
    {
      Win32Api.UnhookWindowsHookEx(hHook);
    }

    private int OnHook(int nCode, Int32 wParam, IntPtr lParam)
    {
      //如果该消息被丢弃（nCode<0）或者没有事件绑定处理程序则不会触发事件
      if (nCode >= 0 && (OnKeyDownEvent != null || OnKeyUpEvent != null))
      {
        var KeyDataFromHook = (Win32Api.KeyboardHookStruct)Marshal.PtrToStructure(lParam, typeof(Win32Api.KeyboardHookStruct));
        var keyData = KeyInterop.KeyFromVirtualKey(KeyDataFromHook.vkCode);

        //WM_KEYDOWN和WM_SYSKEYDOWN消息，将会引发OnKeyDownEvent事件

        if (OnKeyDownEvent != null && (wParam == Win32Api.WM_KEYDOWN || wParam == Win32Api.WM_SYSKEYDOWN))
        {
          OnKeyDownEvent(this, new KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource ?? new HwndSource(0, 0, 0, 0, 0, "", IntPtr.Zero), 0, keyData));
        }
        
        //WM_KEYUP和WM_SYSKEYUP消息，将引发OnKeyUpEvent事件 
        if (OnKeyUpEvent != null && (wParam == Win32Api.WM_KEYUP || wParam == Win32Api.WM_SYSKEYUP))
        {
          OnKeyUpEvent(this, new KeyEventArgs(Keyboard.PrimaryDevice, Keyboard.PrimaryDevice.ActiveSource ?? new HwndSource(0, 0, 0, 0, 0, "", IntPtr.Zero), 0, keyData));
        }
      }

      return Win32Api.CallNextHookEx(hHook, nCode, wParam, lParam);
    }
  }
}
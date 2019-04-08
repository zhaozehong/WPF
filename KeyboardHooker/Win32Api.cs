using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace KeyboardHooker
{
  public class Win32Api
  {
    #region 常数和结构
    public const int WM_KEYDOWN = 0x100;
    public const int WM_KEYUP = 0x101;
    public const int WM_SYSKEYDOWN = 0x104;
    public const int WM_SYSKEYUP = 0x105;
    public const int WH_KEYBOARD_LL = 13;

    [StructLayout(LayoutKind.Sequential)] //声明键盘钩子的封送结构类型 
    public class KeyboardHookStruct
    {
      public int vkCode; //表示一个在1到254间的虚似键盘码 
      public int scanCode; //表示硬件扫描码 
      public int flags;
      public int time;
      public int dwExtraInfo;
    }

    #endregion

    #region Api
    public delegate int HookProc(int nCode, Int32 wParam, IntPtr lParam);

    //安装钩子的函数 
    [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
    public static extern int SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);

    //卸下钩子的函数 
    [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
    public static extern bool UnhookWindowsHookEx(int idHook);

    //下一个钩挂的函数 
    [DllImport("user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
    public static extern int CallNextHookEx(int idHook, int nCode, Int32 wParam, IntPtr lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall)]
    public static extern IntPtr GetModuleHandle(string lpModuleName);

    [DllImport("user32.dll", EntryPoint = "keybd_event")]
    public static extern void SendKeyboardEvent(
      byte bVk, //虚拟键值
      byte bScan,// 一般为0
      int dwFlags, //这里是整数类型 0 为按下，2为释放
      int dwExtraInfo //这里是整数类型 一般情况下设成为0  
    );

    #endregion
  }
}

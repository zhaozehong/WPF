using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;
using System.Windows.Media;

namespace Hexagon.Software.NCGage.UserControls
{
  public class PopupEx : Popup
  {
    public PopupEx()
    {
      this.Loaded += (s, e) => SetMoveWithWindow();
    }

    private void SetMoveWithWindow()
    {
      var window = Window.GetWindow(this);
      if (window == null)
        return;

      window.SizeChanged -= PositionChanged;
      window.LocationChanged -= PositionChanged;
      if (AutoUpdatePosition)
      {
        window.SizeChanged += PositionChanged;
        window.LocationChanged += PositionChanged;
      }
    }
    private void PositionChanged(object sender, EventArgs e)
    {
      try
      {
        if (this.IsOpen)
        {
          var method = typeof(Popup).GetMethod("UpdatePosition", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
          method.Invoke(this, null); // UpdatePosition
        }
      }
      catch { }
    }
    private void UpdateWindow()
    {
      var hwnd = ((HwndSource)PresentationSource.FromVisual(this.Child)).Handle;
      RECT rect;
      if (NativeMethods.GetWindowRect(hwnd, out rect))
      {
        NativeMethods.SetWindowPos(hwnd, Topmost ? -1 : -2, rect.Left, rect.Top, (int)this.Width, (int)this.Height, 0);
      }
    }

    protected override void OnOpened(EventArgs e)
    {
      UpdateWindow();
    }

    #region Dependency Properties
    public bool AutoUpdatePosition
    {
      get { return (bool)GetValue(AutoUpdatePositionProperty); }
      set { SetValue(AutoUpdatePositionProperty, value); }
    }
    public static readonly DependencyProperty AutoUpdatePositionProperty =
        DependencyProperty.Register("AutoUpdatePosition", typeof(bool), typeof(PopupEx), new PropertyMetadata(true, new PropertyChangedCallback(OnAutoUpdatePositionChanged)));
    private static void OnAutoUpdatePositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var popup = d as PopupEx;
      if (popup != null)
        popup.SetMoveWithWindow();
    }

    public bool Topmost
    {
      get { return (bool)GetValue(TopmostProperty); }
      set { SetValue(TopmostProperty, value); }
    }
    public static DependencyProperty TopmostProperty = Window.TopmostProperty.AddOwner(typeof(Popup), new FrameworkPropertyMetadata(false, OnTopmostChanged));
    private static void OnTopmostChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var popup = d as PopupEx;
      if (popup != null)
        popup.UpdateWindow();
    }

    #endregion

    #region imports & definitions
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
      public int Left;
      public int Top;
      public int Right;
      public int Bottom;
    }
    public static class NativeMethods
    {
      [DllImport("user32.dll")]
      [return: MarshalAs(UnmanagedType.Bool)]
      internal static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);
      [DllImport("user32", EntryPoint = "SetWindowPos")]
      internal static extern int SetWindowPos(IntPtr hWnd, int hwndInsertAfter, int x, int y, int cx, int cy, int wFlags);
    }
    #endregion
  }
}

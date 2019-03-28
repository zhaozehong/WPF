using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Windows.Input;

namespace Hexagon.Software.NCGage.UserControls
{
  public class PopupEx : Popup
  {
    public PopupEx()
    {
      //this.MouseLeftButtonDown += KeyboardPopup_MouseLeftButtonDown;
      //this.MouseLeftButtonUp += KeyboardPopup_MouseLeftButtonUp;
      //this.MouseMove += KeyboardPopup_MouseMove;
      //this.Closed += KeyboardPopup_Closed;

      this.Loaded += (s, e) =>
      {
        SetUpdatePosition();
        if (this.SharePosition && !_PopupList.Contains(this))
        {
          _PopupList.Add(this);
          // since IsPin is always false, so we don't need to initialize _PinPosition when startup
        }
      };
    }

    #region move
    private void KeyboardPopup_Closed(object sender, EventArgs e)
    {
      this.HorizontalOffset = 0;
      this.VerticalOffset = 0;
    }
    private Point startPoint;
    private bool isMoving;
    private void KeyboardPopup_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      startPoint = this.Child.PointToScreen(e.GetPosition(this.Child));
      this.CaptureMouse();
    }
    private void KeyboardPopup_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      if(isMoving || this.IsPin)
      {
        SavePostion();
      }
      this.ReleaseMouseCapture();
      isMoving = false;
    }
    private void KeyboardPopup_MouseMove(object sender, MouseEventArgs e)
    {
      if (e.LeftButton == MouseButtonState.Pressed)
      {
        isMoving = true;
        var newPosition = this.Child.PointToScreen(e.GetPosition(this.Child));
        this.HorizontalOffset += (newPosition.X - startPoint.X);
        this.VerticalOffset += (newPosition.Y - startPoint.Y);
        startPoint = newPosition;
      }
    }
    #endregion

    protected override void OnOpened(EventArgs e)
    {
      // set position
      UpdateWindow();

      /********************************ZEHONG: fix bug when Topmost set to true********************************
       * When topmost keyboard is opened: textbox will lost its focus;
       * 1. when keyboard is closed: textbox got focus again, and GotFocus will open keyboard again;
       * 2. opened keyboard makes textbox lost its focus: LostFocus will close the keyboard
       * so, 1 & 2 creates a dead loop.
       *********************************************************************************************************/
      if (this.PlacementTarget.IsFocused == false) //ZEHONG: PlacementTarget must not be null when coming here
        this.PlacementTarget.Focus();
    }

    private void OnIsPinChanged()
    {
      this.SetUpdatePosition();
      this.SavePostion();

      // Apply IsPin to all Popups with SharePostion flag
      if (this.SharePosition)
      {
        Debug.Assert(_PopupList.All(p => p.SharePosition));
        _PopupList.Where(p => p.SharePosition).ToList().ForEach(p => p.IsPin = this.IsPin);
      }
    }
    private void OnSharePositionChanged()
    {
      if (SharePosition)
      {
        if (!_PopupList.Contains(this))
          _PopupList.Add(this);
      }
      else
      {
        if (_PopupList.Contains(this))
          _PopupList.Remove(this);
      }
      this.SavePostion();
    }
    private void SavePostion()
    {
      try
      {
        if (_IsSavingPinPostion)
          return;

        _IsSavingPinPostion = true;
        if (!SharePosition)
          return;

        if (!IsPin)
          _PinPosition = null;

        RECT rect;
        var hwnd = ((HwndSource)PresentationSource.FromVisual(this.Child))?.Handle;
        if (hwnd == null || !NativeMethods.GetWindowRect(hwnd.Value, out rect))
          return;

        _PinPosition = new Point(rect.Left, rect.Top);
      }
      finally
      {
        _IsSavingPinPostion = false;
      }
    }

    // Update Position
    private void SetUpdatePosition()
    {
      var window = Window.GetWindow(this);
      if (window == null)
        return;

      window.SizeChanged -= UpdatePosition;
      window.LocationChanged -= UpdatePosition;
      if (!IsPin)
      {
        window.SizeChanged += UpdatePosition;
        window.LocationChanged += UpdatePosition;
      }
    }
    private void UpdatePosition(object sender, EventArgs e)
    {
      try
      {
        if (this.IsOpen)
        {
          var method = typeof(Popup).GetMethod("UpdatePosition", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
          method.Invoke(this, null);
        }
      }
      catch { }
    }

    // Set position & apply Topmost property
    private void UpdateWindow()
    {
      var hwnd = ((HwndSource)PresentationSource.FromVisual(this.Child))?.Handle;
      if (hwnd == null)
        return;

      RECT rect;
      if (IsPin && SharePosition && _PinPosition.HasValue)
      {
        rect.Left = (int)_PinPosition.Value.X;
        rect.Top = (int)_PinPosition.Value.Y;
      }
      else
      {
        if (!NativeMethods.GetWindowRect(hwnd.Value, out rect))
          return;
      }
      NativeMethods.SetWindowPos(hwnd.Value, Topmost ? -1 : -2, rect.Left, rect.Top, (int)this.Width, (int)this.Height, 0);
    }

    #region Dependency Properties
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
        popup.UpdateWindow(); // apply Topmost
    }

    // ZEHONG: set IsPin to private to prevent it from being initialized to true in XAML
    private bool IsPin
    {
      get { return (bool)GetValue(IsPinProperty); }
      set { SetValue(IsPinProperty, value); }
    }
    public static readonly DependencyProperty IsPinProperty =
        DependencyProperty.Register("IsPin", typeof(bool), typeof(PopupEx), new PropertyMetadata(false, OnIsPinChanged));
    private static void OnIsPinChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var popup = d as PopupEx;
      if (popup != null)
        popup.OnIsPinChanged();
    }

    public bool SharePosition
    {
      get { return (bool)GetValue(SharePositionProperty); }
      set { SetValue(SharePositionProperty, value); }
    }
    public static readonly DependencyProperty SharePositionProperty =
        DependencyProperty.Register("SharePosition", typeof(bool), typeof(PopupEx), new PropertyMetadata(true, OnSharePositionChanged));
    private static void OnSharePositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var popup = d as PopupEx;
      if (popup != null)
        popup.OnSharePositionChanged();
    }

    #endregion

    #region static
    private static List<PopupEx> _PopupList = new List<PopupEx>();
    private static bool _IsSavingPinPostion = false;
    private static Point? _PinPosition = null;
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

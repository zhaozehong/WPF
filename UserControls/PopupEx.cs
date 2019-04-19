using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Windows.Input;
using System.Windows.Controls;
using Hexagon.Software.NCGage.HelperLib;
using System.Windows.Media;

namespace Hexagon.Software.NCGage.UserControls
{
  public class PopupEx : Popup
  {
    public PopupEx()
    {
      this.LayoutUpdated += PopupEx_LayoutUpdated;
      CompositionTarget.Rendering += CompositionTarget_Rendering;
    }

    protected override void OnOpened(EventArgs e)
    {
      // set position
      if (!_hasOpenedBefore)
        SetWindowPosition(); // to set topmost property
      _hasOpenedBefore = true;

      if (this.IsPin)
      {
        _backupPlacement = this.Placement;
        this.Placement = PlacementMode.Absolute;
      }

      /********************************ZEHONG: fix bug when Topmost set to true********************************
       * When topmost keyboard is opened: textbox will lost its focus;
       * 1. when keyboard is closed: textbox got focus again, and GotFocus will open keyboard again;
       * 2. opened keyboard makes TextBox lost its focus: LostFocus will close the keyboard
       * so, 1 & 2 creates a dead loop.
       *********************************************************************************************************/
      if (this.PlacementTarget.IsFocused == false) //ZEHONG: PlacementTarget must not be null when coming here
        this.PlacementTarget.Focus();
    }

    private void OnIsPinChanged()
    {
      this.ApplyWindowEvents();

      if (this.IsPin)
      {
        _backupPlacement = this.Placement;

        var topLeftPoint = this.Child.PointToScreen(Helpers.ZeroPoint);
        this.HorizontalOffset = Helpers.PixelsToDIU((int)topLeftPoint.X);
        this.VerticalOffset = Helpers.PixelsToDIU((int)topLeftPoint.Y, false);
        this.Placement = PlacementMode.Absolute;
      }
      else
      {
        this.Placement = _backupPlacement;
        this.HorizontalOffset = 0;
        this.VerticalOffset = 0;
      }
      UpdateOffsetLimit();

      // Apply IsPin to all Popups with SharePostion flag
      if (this.SharePosition)
      {
        Debug.Assert(_PopupList.All(p => p.SharePosition));
        _PopupList.Where(p => p.SharePosition).ToList().ForEach(p =>
        {
          p.IsPin = this.IsPin;
          p.HorizontalOffset = this.HorizontalOffset;
          p.VerticalOffset = this.VerticalOffset;
        });
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
    }

    // Update Position
    private bool ApplyWindowEvents()
    {
      var window = Window.GetWindow(this);
      if (window == null)
        return false;

      window.SizeChanged -= Window_Changed;
      window.LocationChanged -= Window_Changed;
      if (!IsPin)
      {
        window.SizeChanged += Window_Changed;
        window.LocationChanged += Window_Changed;
      }
      return true;
    }
    private void Window_Changed(object sender, EventArgs e)
    {
      try
      {
        if (this.IsOpen && !this.IsPin)
        {
          var method = typeof(Popup).GetMethod("UpdatePosition", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
          method.Invoke(this, null);
        }
      }
      catch { }
    }

    // Set position & apply Topmost property
    private void SetWindowPosition()
    {
      var hwnd = ((HwndSource)PresentationSource.FromVisual(this.Child))?.Handle;
      if (hwnd == null)
        return;

      RECT rect;
      if (NativeMethods.GetWindowRect(hwnd.Value, out rect))
        NativeMethods.SetWindowPos(hwnd.Value, Topmost ? -1 : -2, rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top, 0);
    }
    private void UpdateOffsetLimit()
    {
      if (!(this.Child is FrameworkElement child))
        return;

      try
      {
        var workArea = Helpers.GetWorkArea(this);
        var topLeftPoint = this.Child.PointToScreen(Helpers.ZeroPoint);
        var bottomRightPoint = this.Child.PointToScreen(new Point(child.ActualWidth, child.ActualHeight));
        if (this.Placement == PlacementMode.Absolute)
        {
          _minHorOffset = _minVerOffset = 0;
          _maxHorOffset = workArea.Size.Width - Helpers.PixelsToDIU((int)(bottomRightPoint.X - topLeftPoint.X));
          _maxVerOffset = workArea.Size.Height - Helpers.PixelsToDIU((int)(bottomRightPoint.Y - topLeftPoint.Y), false);
        }
        else
        {
          _minHorOffset = Helpers.PixelsToDIU(0 - (int)topLeftPoint.X) + this.HorizontalOffset;
          _minVerOffset = Helpers.PixelsToDIU(0 - (int)topLeftPoint.Y, false) + this.VerticalOffset;

          _maxHorOffset = workArea.Size.Width - Helpers.PixelsToDIU((int)bottomRightPoint.X) + this.HorizontalOffset;
          _maxVerOffset = workArea.Size.Height - Helpers.PixelsToDIU((int)bottomRightPoint.Y, false) + this.VerticalOffset;
        }
      }
      catch (Exception) { }
    }

    private void PopupEx_LayoutUpdated(object sender, EventArgs e)
    {
      if (ApplyWindowEvents())
      {
        if (this.SharePosition && !_PopupList.Contains(this))
          _PopupList.Add(this);
        this.LayoutUpdated -= PopupEx_LayoutUpdated;
      }
    }
    private void CompositionTarget_Rendering(object sender, EventArgs e)
    {
      if (this.IsOpen)
        UpdateOffsetLimit();
    }

    #region Move Popup
    protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
    {
      base.OnPreviewMouseLeftButtonDown(e);
      if (e.OriginalSource == null || e.OriginalSource.GetType() != typeof(Grid))
        return;

      startPoint = this.Child.PointToScreen(e.GetPosition(this.Child));
      this.CaptureMouse();
      e.Handled = true;
    }
    protected override void OnMouseMove(MouseEventArgs e)
    {
      base.OnMouseMove(e);
      if (!startPoint.HasValue)
        return;

      var newPosition = this.Child.PointToScreen(e.GetPosition(this.Child));

      var hOffset = this.HorizontalOffset + Helpers.PixelsToDIU((int)(newPosition.X - startPoint.Value.X));
      this.HorizontalOffset = Math.Min(_maxHorOffset, Math.Max(_minHorOffset, hOffset));

      var vOffset = this.VerticalOffset + Helpers.PixelsToDIU((int)(newPosition.Y - startPoint.Value.Y), false);
      this.VerticalOffset = Math.Min(_maxVerOffset, Math.Max(_minVerOffset, vOffset));

      startPoint = newPosition;
    }
    protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
    {
      base.OnPreviewMouseLeftButtonUp(e);

      if (startPoint.HasValue)
      {
        this.ReleaseMouseCapture();
        startPoint = null;
        e.Handled = true; // disable keyboard button input
      }
    }
    protected override void OnClosed(EventArgs e)
    {
      base.OnClosed(e);
      if (!this.IsPin)
      {
        this.HorizontalOffset = 0;
        this.VerticalOffset = 0;
      }
    }

    #endregion

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
        popup.SetWindowPosition(); // apply Topmost
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

    #region Variables
    private bool _hasOpenedBefore = false;
    private PlacementMode _backupPlacement = PlacementMode.Bottom;
    private double _minVerOffset, _maxVerOffset, _minHorOffset, _maxHorOffset;
    private Point? startPoint = null;

    #endregion

    private static List<PopupEx> _PopupList = new List<PopupEx>();

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

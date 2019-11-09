using System;
using System.Linq;
using System.Windows;
using System.Diagnostics;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Interop;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Controls.Primitives;
using Hexagon.Software.NCGage.HelperLib;

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
      this.Placement = PlacementMode.Absolute;

      //if (!_hasOpenedBefore)
        SetTopmostStatus(); // to set topmost property
      _hasOpenedBefore = true;

      /********************************ZEHONG: fix bug when Topmost set to true********************************
       * When topmost keyboard is opened: textbox will lost its focus;
       * 1. when keyboard is closed: textbox got focus again, and GotFocus will open keyboard again;
       * 2. opened keyboard makes TextBox lost its focus: LostFocus will close the keyboard
       * so, 1 & 2 creates a dead loop.
       *********************************************************************************************************/
      if (this.PlacementTarget.IsFocused == false) //ZEHONG: PlacementTarget must not be null when coming here
        this.PlacementTarget.Focus();

      SetUnpinPosition();
    }

    private void OnIsPinChanged()
    {
      if (this.IsPin)
      {
        var topLeftPoint = this.Child.PointToScreen(Helpers.ZeroPoint);
        this.HorizontalOffset = Helpers.PixelsToDIU((int)topLeftPoint.X);
        this.VerticalOffset = Helpers.PixelsToDIU((int)topLeftPoint.Y, false);
      }
      else
      {
        SetUnpinPosition();
      }

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

    private bool ApplyWindowEvents()
    {
      var window = Window.GetWindow(this);
      if (window == null)
        return false;

      window.SizeChanged -= Window_Changed;
      window.SizeChanged += Window_Changed;

      window.LocationChanged -= Window_Changed;
      window.LocationChanged += Window_Changed;

      return true;
    }
    private void Window_Changed(object sender, EventArgs e)
    {
      SetUnpinPosition();
    }

    private void SetTopmostStatus()
    {
      var dgCell = Helpers.FindParent<DataGridCell>(this.PlacementTarget);
      if (dgCell != null)
        return;

      var hwnd = ((HwndSource)PresentationSource.FromVisual(this.Child))?.Handle;
      if (hwnd == null)
        return;

      RECT rect;
      if (NativeMethods.GetWindowRect(hwnd.Value, out rect))
        NativeMethods.SetWindowPos(hwnd.Value, Topmost ? -1 : -2, rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top, 0);
    }
    private void UpdateOffsetLimit()
    {
      try
      {
        if (!(this.Child is FrameworkElement child))
          return;

        var currentSize = new Size(child.ActualWidth, child.ActualHeight);
        if (_oldSize.Equals(currentSize))
          return;

        var workArea = Helpers.GetWorkArea(this);
        var topLeftPoint = this.Child.PointToScreen(Helpers.ZeroPoint);
        var bottomRightPoint = this.Child.PointToScreen(new Point(child.ActualWidth, child.ActualHeight));

        _popupWidth = Helpers.PixelsToDIU((int)(bottomRightPoint.X - topLeftPoint.X));
        _popupHeight = Helpers.PixelsToDIU((int)(bottomRightPoint.Y - topLeftPoint.Y), false);

        _maxHorOffset = workArea.Size.Width - _popupWidth;
        _maxVerOffset = workArea.Size.Height - _popupHeight;

        _oldSize = currentSize;
        SetUnpinPosition();
      }
      catch (Exception)
      {
        _oldSize = Size.Empty;
      }
    }
    private void SetUnpinPosition()
    {
      try
      {
        if (!this.IsOpen || this.IsPin || !(PlacementTarget is FrameworkElement placementTarget))
          return;

        var placements = new List<KeyboardPlacementModes>();
        if (this.KeyboardPlacement == KeyboardPlacementModes.Auto)
        {
          // Zehong: don't change the order as it's required by Mazak
          placements.Add(KeyboardPlacementModes.BottomRight);
          placements.Add(KeyboardPlacementModes.TopRight);
          placements.Add(KeyboardPlacementModes.BottomLeft);
          placements.Add(KeyboardPlacementModes.TopLeft);
          placements.Add(KeyboardPlacementModes.Top);
          placements.Add(KeyboardPlacementModes.Top2);
          placements.Add(KeyboardPlacementModes.BottomRightSreen);
        }
        else
        {
          placements.Add(this.KeyboardPlacement);
        }

        double hOffset = double.NaN, vOffset = double.NaN;
        foreach (var placement in placements)
        {
          switch (placement)
          {
            case KeyboardPlacementModes.BottomRight:
              var bottomRightPoint = placementTarget.PointToScreen(new Point(placementTarget.ActualWidth, placementTarget.ActualHeight));
              hOffset = Helpers.PixelsToDIU((int)bottomRightPoint.X);
              vOffset = Helpers.PixelsToDIU((int)bottomRightPoint.Y, false);
              break;
            case KeyboardPlacementModes.BottomLeft:
              var bottomLeftPoint = placementTarget.PointToScreen(new Point(0, placementTarget.ActualHeight));
              hOffset = Helpers.PixelsToDIU((int)bottomLeftPoint.X) - _popupWidth;
              vOffset = Helpers.PixelsToDIU((int)bottomLeftPoint.Y, false);
              break;
            case KeyboardPlacementModes.TopLeft:
              var topLeftPoint = placementTarget.PointToScreen(Helpers.ZeroPoint);
              hOffset = Helpers.PixelsToDIU((int)topLeftPoint.X) - _popupWidth;
              vOffset = Helpers.PixelsToDIU((int)topLeftPoint.Y, false) - _popupHeight;
              break;
            case KeyboardPlacementModes.TopRight:
              var topRightPoint = placementTarget.PointToScreen(new Point(placementTarget.ActualWidth, 0));
              hOffset = Helpers.PixelsToDIU((int)topRightPoint.X);
              vOffset = Helpers.PixelsToDIU((int)topRightPoint.Y, false) - _popupHeight;
              break;
            case KeyboardPlacementModes.Top:
              topLeftPoint = placementTarget.PointToScreen(Helpers.ZeroPoint);
              hOffset = Helpers.PixelsToDIU((int)topLeftPoint.X);
              vOffset = Helpers.PixelsToDIU((int)topLeftPoint.Y, false) - _popupHeight;
              break;
            case KeyboardPlacementModes.Top2:
              topLeftPoint = placementTarget.PointToScreen(Helpers.ZeroPoint);
              hOffset = SystemParameters.WorkArea.Width - _popupWidth;
              vOffset = Helpers.PixelsToDIU((int)topLeftPoint.Y, false) - _popupHeight;
              break;
            case KeyboardPlacementModes.BottomRightSreen:
              hOffset = SystemParameters.WorkArea.Width - _popupWidth;
              vOffset = SystemParameters.WorkArea.Height - _popupHeight;
              break;
          }
          if ((!double.IsNaN(hOffset) && hOffset >= 0 && hOffset <= _maxHorOffset) && (!double.IsNaN(vOffset) && vOffset >= 0 && vOffset <= _maxVerOffset))
            break;
        }

        if (!double.IsNaN(hOffset)) this.HorizontalOffset = Math.Min(_maxHorOffset, Math.Max(0, hOffset));
        if (!double.IsNaN(vOffset)) this.VerticalOffset = Math.Min(_maxVerOffset, Math.Max(0, vOffset));
      }
      catch (Exception) { }
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
      this.HorizontalOffset = Math.Min(_maxHorOffset, Math.Max(0, hOffset));

      var vOffset = this.VerticalOffset + Helpers.PixelsToDIU((int)(newPosition.Y - startPoint.Value.Y), false);
      this.VerticalOffset = Math.Min(_maxVerOffset, Math.Max(0, vOffset));

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
        popup.SetTopmostStatus(); // apply Topmost
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

    public KeyboardPlacementModes KeyboardPlacement
    {
      get { return (KeyboardPlacementModes)GetValue(KeyboardPlacementProperty); }
      set { SetValue(KeyboardPlacementProperty, value); }
    }
    public static readonly DependencyProperty KeyboardPlacementProperty = DependencyProperty.Register("KeyboardPlacement", typeof(KeyboardPlacementModes), typeof(PopupEx), new PropertyMetadata(KeyboardPlacementModes.Auto));

    #endregion

    #region Variables
    private bool _hasOpenedBefore = false;
    private double _maxVerOffset, _maxHorOffset;
    private Point? startPoint = null;
    private Size _oldSize = Size.Empty;
    private double _popupWidth = double.NaN;
    private double _popupHeight = double.NaN;
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
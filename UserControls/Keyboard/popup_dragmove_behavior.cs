using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace Hexagon.Software.NCGage.UserControls
{
  public class popup_dragmove_behavior : Behavior<Popup>
  {
    public bool _mouse_down;
    Point _old_pos;

    Point _orignal_pos;
    double max_vertical_offset;
    protected override void OnAttached()
    {
      base.OnAttached();

      AssociatedObject.MouseLeftButtonDown += (sender, e) =>
      {
        mouse_left_button_down(sender, e);
      };
      AssociatedObject.MouseLeftButtonUp += (sender, e) =>
      {
        mouse_left_button_up(sender, e);
      };
      AssociatedObject.MouseMove += (sender, e) =>
      {
        mouse_move(sender, e);
      };
      AssociatedObject.Closed += (sender, e) =>
      {
        popup_close(sender, e);
      };
    }

    protected override void OnDetaching()
    {
      base.OnDetaching();
      AssociatedObject.MouseLeftButtonDown -= (sender, e) =>
      {
        mouse_left_button_down(sender, e);
      };
      AssociatedObject.MouseLeftButtonUp -= (sender, e) =>
      {
        mouse_left_button_up(sender, e);
      };
      AssociatedObject.MouseMove -= (sender, e) =>
      {
        mouse_move(sender, e);
      };
      AssociatedObject.Closed -= (sender, e) =>
      {
        popup_close(sender, e);
      };
    }

    void mouse_left_button_down(Object sender, MouseButtonEventArgs e)
    {
      _mouse_down = true;
      if (AssociatedObject.VerticalOffset == 0)
      {
        _orignal_pos = AssociatedObject.Child.PointToScreen(new Point(AssociatedObject.ActualWidth, 0));
        max_vertical_offset = 0 - _orignal_pos.Y;
      }
      _old_pos = AssociatedObject.Child.PointToScreen(e.GetPosition(AssociatedObject.Child));
      AssociatedObject.Child.CaptureMouse();
    }

    void mouse_move(Object sender, MouseEventArgs e)
    {
      if (!_mouse_down)
      {
        return;
      }
      var child_pos = e.GetPosition(AssociatedObject.Child);
      var new_pos = AssociatedObject.Child.PointToScreen(child_pos);
      var offset = new_pos - _old_pos;
      _old_pos = new_pos;
      AssociatedObject.HorizontalOffset += offset.X;
      var new_VerticalOffset = AssociatedObject.VerticalOffset + offset.Y;
      if (new_VerticalOffset < max_vertical_offset)
      {
        new_VerticalOffset = max_vertical_offset;
      }
      AssociatedObject.VerticalOffset = new_VerticalOffset;
    }

    void mouse_left_button_up(Object sender, MouseButtonEventArgs e)
    {
      _mouse_down = false;
      AssociatedObject.Child.ReleaseMouseCapture();
    }

    void popup_close(Object sender, EventArgs e)
    {
      AssociatedObject.HorizontalOffset = 0;
      AssociatedObject.VerticalOffset = 0;
    }
  }
}

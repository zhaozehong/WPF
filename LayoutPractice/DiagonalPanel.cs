﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace LayoutPractice
{
  public class DiagonalPanel : Panel
  {
    protected override Size MeasureOverride(Size availableSize)
    {
      var mySize = new Size();
      foreach (UIElement child in this.InternalChildren)
      {
        child.Measure(availableSize);
        mySize.Width += child.DesiredSize.Width;
        mySize.Height += child.DesiredSize.Height;
      }

      return mySize;
    }
    protected override Size ArrangeOverride(Size finalSize)
    {
      var location = new Point();
      foreach (UIElement child in this.InternalChildren)
      {
        child.Arrange(new Rect(location, child.DesiredSize));
        location.X += child.DesiredSize.Width;
        location.Y += child.DesiredSize.Height;
      }

      return finalSize;
    }
  }
}

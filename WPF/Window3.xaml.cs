using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace JM.Software.WPF
{
  public partial class Window3 : Window
  {
    public Window3()
    {
      InitializeComponent();
      
    }

    protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
    {
      base.OnMouseLeftButtonUp(e);
    }
    private void Thumb_DragDelta(object sender, System.Windows.Controls.Primitives.DragDeltaEventArgs e)
    {
      Thumb myThumb = (Thumb)sender;
      double nTop = Canvas.GetTop(myThumb) + e.VerticalChange;
      double nLeft = Canvas.GetLeft(myThumb) + e.HorizontalChange;
      //防止Thumb控件被拖出容器。
      if (nTop <= 0)
        nTop = 0;
      if (nTop >= (g.Height - myThumb.Height))
        nTop = g.Height - myThumb.Height;
      if (nLeft <= 0)
        nLeft = 0;
      if (nLeft >= (g.Width - myThumb.Width))
        nLeft = g.Width - myThumb.Width;
      Canvas.SetTop(myThumb, nTop);
      Canvas.SetLeft(myThumb, nLeft);
      tt.Text = "Top:" + nTop.ToString() + "\nLeft:" + nLeft.ToString();
    }

    private void Thumb_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      e.Handled = false;
    }
  }
}

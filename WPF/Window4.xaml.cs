using System;
using System.Windows;

namespace JM.Software.WPF
{
  public partial class Window4 : Window
  {
    
    public Window4()
    {
      InitializeComponent();
    }
    public void ResetPopUp()
    {
      Random random = new Random();
      MyPopUp.PlacementRectangle = new Rect(new Point(random.NextDouble() / 10, 0), new Size(175, 25));
      //MyPopUp.PlacementRectangle = new Rect(StartPoint, new Size(75, 25));
    }
    private void Window_LocationChanged(object sender, EventArgs e)
    {
      //StartPoint = _TheText.PointToScreen(new Point(0, 0));
      ResetPopUp();
    }
    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      //StartPoint = _TheText.PointToScreen(new Point(0, 0));
    }
    private void Rectangle_LayoutUpdated(object sender, EventArgs e)
    {
      ResetPopUp();
    }


    public Point StartPoint = new Point(0, 0);
    public Point EndPoint = new Point(0, 0);
  }
}

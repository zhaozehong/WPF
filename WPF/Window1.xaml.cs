using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WPF.CustomControls;

namespace WPF
{
  /// <summary>
  /// Interaction logic for Window1.xaml
  /// </summary>
  public partial class Window1 : Window
  {
    public Window1()
    {
      InitializeComponent();
    }

    private void FanucButton_Click(object sender, RoutedEventArgs e)
    {
      var button = sender as FanucButton;
      if (button != null)
      {
        //statisticsButton.LicenseMode = statisticsButton.LicenseMode == LicenseModes.License ? LicenseModes.Disabled : LicenseModes.License;
        statisticsButton.IsEnabled = !statisticsButton.IsEnabled;
      }
      //MessageBox.Show(String.Format("{0} is clicked.", button.FunctionName));
    }
  }
}

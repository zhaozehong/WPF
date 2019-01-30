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
using System.Windows.Navigation;
using System.Windows.Shapes;
using WPF.ViewModel;
namespace WPF
{
  public partial class MainWindow : Window
  {
    public MainWindow()
    {
      InitializeComponent();

      _vm = new MainWindowViewModel();
      this.DataContext = _vm;
    }


    private MainWindowViewModel _vm;

    private void Grid_RequestNavigate(object sender, RequestNavigateEventArgs e)
    {
      MessageBox.Show(e.Uri.ToString(), "Navigate");
    }
  }
}

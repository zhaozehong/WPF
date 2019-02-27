using Helper;
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
  public partial class Window1 : Window
  {
    public Window1()
    {
      this.ViewModel = new Window1ViewModel();
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

    public Window1ViewModel ViewModel { get; private set; }
  }
  public class Window1ViewModel : NotifyPropertyChanged
  {
    public Window1ViewModel()
    {
      this.Model = new DataObject() { Name = "Model" };
      this.UpdateCommand = new RelayCommand(UpdateText);
    }

    private void UpdateText()
    {
      this.Model.Name = DateTime.Now.ToString();
    }

    public RelayCommand UpdateCommand { get; private set; }
    public DataObject Model
    {
      get { return _model; }
      set
      {
        if (_model != value)
        {
          _model = value;
          this.RaisePropertyChanged(() => Model);
        }
      }
    }

    private DataObject _model = null;
  }
  public class DataObject: NotifyPropertyChanged
  {
    private String _name;
    public System.String Name
    {
      get { return _name; }
      set
      {
        _name = value;
        this.RaisePropertyChanged(() => Name);
      }
    }
  }
}

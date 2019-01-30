﻿using System;
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

      var cv = CollectionViewSource.GetDefaultView(_vm.People);
      //cv.Filter = MyFilter;
      cv.GroupDescriptions.Add(new PropertyGroupDescription("FirstName"));
    }
    private bool MyFilter(object item)
    {
      var src = item as PersonItemViewModel;
      return src.FirstName != "Ian";
    }


    private MainWindowViewModel _vm;

    private void Grid_RequestNavigate(object sender, RequestNavigateEventArgs e)
    {
      MessageBox.Show(e.Uri.ToString(), "Navigate");
    }

    private void DataGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
    {
      switch (e.PropertyName)
      {
        case "FirstName":
          e.Column.Header = "First Name"; // change column header
          e.Column.DisplayIndex = 1; // not recommended, it will easily cause OutOfRange exception
          break;
        case "Model":
          e.Cancel = true; // prevent from generating column
          break;
      };
    }
  }
}

using System.Windows;

namespace JM.Software.WPF
{
  public partial class Window2 : Window
  {
    public Window2()
    {
      InitializeComponent();
      this.tbInput.Focus();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
      var dialog = new TestDialog();
      dialog.Show();
    }
  }
}

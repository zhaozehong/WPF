using System;
using System.Windows;
using System.ComponentModel;

namespace Hexagon.Software.NCGage.UserControls
{
  public partial class MessageBoxEx : Window, INotifyPropertyChanged
  {
    public MessageBoxEx()
    {
      InitializeComponent();
    }

    private void btnOk_Click(object sender, RoutedEventArgs e)
    {
      this.Close();
    }

    #region INotifyPropertyChanged Members
    public event PropertyChangedEventHandler PropertyChanged;
    private void SendPropertyChanged(String propertyName)
    {
      if (PropertyChanged != null)
        this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
    }
    #endregion

    public string Prompt
    {
      get { return _prompt; }
      set
      {
        _prompt = value;
        this.SendPropertyChanged(nameof(Prompt));
      }
    }
    public string Details
    {
      get { return _details; }
      set
      {
        _details = value;
        this.SendPropertyChanged(nameof(Details));
        runDetails.Text = value;
      }
    }
    public bool HasDetails
    {
      get { return _hasDetails; }
      set
      {
        _hasDetails = value;
        this.SendPropertyChanged(nameof(HasDetails));
      }
    }

    private string _prompt = null;
    private string _details = null;
    private bool _hasDetails = false;
  }
}
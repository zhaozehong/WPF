using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using WPF.Helper;

namespace WPF.UserControls
{
  public class KeyboardControl : UserControl, INotifyPropertyChanged
  {
    protected void FireSwitchedEvent(object sender, EventArgs e)
    {
      if (Switched != null)
        Switched(sender, e);
    }
    protected void FireClosedEvent(object sender, EventArgs e)
    {
      if (Closed != null)
        Closed(sender, e);
    }

    #region INotifyPropertyChanged
    public event PropertyChangedEventHandler PropertyChanged;
    private void RaisePropertyChanged(String propertyName)
    {
      PropertyChangedEventHandler handler = null;
      lock (this)
      {
        handler = PropertyChanged;
      }
      if (handler != null)
        handler(this, new PropertyChangedEventArgs(propertyName));
    }
    #endregion

    public event EventHandler Switched;
    public event EventHandler Closed;

    private Double _displayScreenWidth = 0;
    public Double DisplayScreenWidth
    {
      get { return _displayScreenWidth; }
      set
      {
        if (!Helpers.IsNullOrZero(_displayScreenWidth - value))
        {
          _displayScreenWidth = value;
          this.RaisePropertyChanged(nameof(DisplayScreenWidth));
        }
      }
    }
  }
}

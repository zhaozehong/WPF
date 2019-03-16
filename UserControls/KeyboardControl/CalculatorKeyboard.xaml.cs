using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using WPF.Helper;

namespace WPF.UserControls
{
  public partial class CalculatorKeyboard : UserControl, INotifyPropertyChanged
  {
    public CalculatorKeyboard()
    {
      InitializeComponent();
      this.ViewModel = this.DataContext as CalculatorKeyboardViewModel;
      this.ViewModel.PropertyChanged += ViewModel_PropertyChanged;
    }

    private void OnButtonSizeChanged()
    {
      this.UpdateDisplayScreenSize();
      this.ButtonFontSize = (Int32)(this.ButtonSize / 3);
    }
    private void UpdateDisplayScreenSize()
    {
      this.DisplayScreenWidth = this.ButtonSize * (this.ViewModel.KeyboardMode == KeyboardModes.Calculator ? 9 : 4) + 16;
    }

    private void btnClose_Click(object sender, RoutedEventArgs e)
    {
      if (Closed != null)
        Closed(this, null);
    }
    private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == nameof(ViewModel.KeyboardMode))
      {
        this.UpdateDisplayScreenSize();
      }
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

    public event EventHandler Closed;

    #region Properties
    protected CalculatorKeyboardViewModel ViewModel { get; private set; }
    public Int32 ButtonFontSize
    {
      get { return _buttonFontSize; }
      set
      {
        if (_buttonFontSize != value)
        {
          _buttonFontSize = value;
          this.RaisePropertyChanged(nameof(ButtonFontSize));
        }
      }
    }
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

    #endregion

    #region Dependency Properties
    public Double ButtonSize
    {
      get { return (Double)GetValue(ButtonSizeProperty); }
      set { SetValue(ButtonSizeProperty, value); }
    }

    public static readonly DependencyProperty ButtonSizeProperty =
        DependencyProperty.Register("ButtonSize", typeof(Double), typeof(CalculatorKeyboard), new PropertyMetadata(100.0, new PropertyChangedCallback(OnButtonSizeChanged)));

    private static void OnButtonSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var control = d as CalculatorKeyboard;
      if (control != null)
        control.OnButtonSizeChanged();
    }

    #endregion

    #region Variables
    private Int32 _buttonFontSize = 16;
    private Double _displayScreenWidth = 50;
    #endregion
  }
}

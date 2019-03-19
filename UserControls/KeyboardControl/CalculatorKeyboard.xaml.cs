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

      OnButtonSizeChanged();
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
    private void btnPin_Click(object sender, RoutedEventArgs e)
    {
      this.IsPin = !IsPin;
    }
    private void btnCLR_Click(object sender, RoutedEventArgs e)
    {
      if (this.TargetElement != null)
        this.TargetElement.Clear();
    }
    private void btnEnter_Click(object sender, RoutedEventArgs e)
    {
      if (this.TargetElement != null)
        this.TargetElement.Text = this.ViewModel.InputValue;
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
    public double DisplayScreenWidth
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
    public double ButtonSize
    {
      get { return (Double)GetValue(ButtonSizeProperty); }
      set { SetValue(ButtonSizeProperty, value); }
    }
    public static readonly DependencyProperty ButtonSizeProperty = DependencyProperty.Register("ButtonSize", typeof(double), typeof(CalculatorKeyboard), new PropertyMetadata(60.0, new PropertyChangedCallback(OnButtonSizeChanged)));
    private static void OnButtonSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var control = d as CalculatorKeyboard;
      if (control != null)
        control.OnButtonSizeChanged();
    }

    public TextBox TargetElement
    {
      get { return (TextBox)GetValue(TargetElementProperty); }
      set { SetValue(TargetElementProperty, value); }
    }
    public static readonly DependencyProperty TargetElementProperty = DependencyProperty.Register("TargetElement", typeof(TextBox), typeof(CalculatorKeyboard), new PropertyMetadata(null));

    public bool IsPin
    {
      get { return (Boolean)GetValue(IsPinProperty); }
      set { SetValue(IsPinProperty, value); }
    }
    public static readonly DependencyProperty IsPinProperty = DependencyProperty.Register("IsPin", typeof(bool), typeof(CalculatorKeyboard), new PropertyMetadata(false));

    #endregion

    #region Variables
    private Int32 _buttonFontSize = 16;
    private Double _displayScreenWidth = 50;

    #endregion
  }
}

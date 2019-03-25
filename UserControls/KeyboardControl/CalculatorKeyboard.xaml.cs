using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using Hexagon.Software.NCGage.HelperLib;

namespace Hexagon.Software.NCGage.UserControls
{
  public partial class CalculatorKeyboard : UserControl, INotifyPropertyChanged
  {
    public CalculatorKeyboard()
    {
      InitializeComponent();
      this.ViewModel = this.DataContext as CalculatorKeyboardViewModel;
      this.ViewModel.PropertyChanged += ViewModel_PropertyChanged;

      this.OnStartupKeyboardTypeChanged();
      this.OnResetOnCalculationChanged();
      this.OnButtonSizeChanged();

      this.Loaded += (s, e) =>
       {
#if !DEBUG
         this.IsPin = false;
#endif
       };
    }

    private void UpdateDisplayScreenSize()
    {
      var columns = this.ViewModel.KeyboardType == KeyboardTypes.Calculator ? 9 : 4;
      this.DisplayScreenWidth = this.ButtonSize * columns + this.ButtonMargin * 2 * (columns - 1);
    }
    private void UpdateButtonFontSize()
    {
      this.ButtonFontSize = (Int32)(this.ButtonSize / 3);
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
      if (this.InputTarget != null)
        this.InputTarget.Clear();
    }
    private void btnEnter_Click(object sender, RoutedEventArgs e)
    {
      if (this.InputTarget != null)
      {
        this.InputTarget.Clear();
        Helpers.RaiseTextInputEvent(this.InputTarget, this.ViewModel.InputValue);
      }
    }

    private void OnStartupKeyboardTypeChanged()
    {
      this.ViewModel.KeyboardType = this.StartupKeyboardType;
    }
    private void OnButtonSizeChanged()
    {
      this.UpdateDisplayScreenSize();
      this.UpdateButtonFontSize();
    }
    private void OnButtonMarginChanged()
    {
      this.UpdateDisplayScreenSize();
    }
    private void OnResetOnCalculationChanged()
    {
      this.ViewModel.IsResetAfterCalculation = this.ResetOnCalculation;
    }
    private void ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      if (e.PropertyName == nameof(ViewModel.KeyboardType))
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
    public TextBox InputTarget
    {
      get { return (TextBox)GetValue(InputTargetProperty); }
      set { SetValue(InputTargetProperty, value); }
    }
    public static readonly DependencyProperty InputTargetProperty = DependencyProperty.Register("InputTarget", typeof(TextBox), typeof(CalculatorKeyboard), new PropertyMetadata(null));

    public KeyboardTypes StartupKeyboardType
    {
      get { return (KeyboardTypes)GetValue(StartupKeyboardTypeProperty); }
      set { SetValue(StartupKeyboardTypeProperty, value); }
    }
    public static readonly DependencyProperty StartupKeyboardTypeProperty = DependencyProperty.Register("StartupKeyboardType", typeof(KeyboardTypes), typeof(CalculatorKeyboard), new PropertyMetadata(KeyboardTypes.Number, new PropertyChangedCallback(OnStartupKeyboardTypeChanged)));
    private static void OnStartupKeyboardTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var control = d as CalculatorKeyboard;
      if (control != null)
        control.OnStartupKeyboardTypeChanged();
    }

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

    public double ButtonMargin
    {
      get { return (double)GetValue(ButtonMarginProperty); }
      set { SetValue(ButtonMarginProperty, value); }
    }
    public static readonly DependencyProperty ButtonMarginProperty = DependencyProperty.Register("ButtonMargin", typeof(double), typeof(CalculatorKeyboard), new PropertyMetadata(1.0, new PropertyChangedCallback(OnButtonMarginChanged)));
    private static void OnButtonMarginChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var control = d as CalculatorKeyboard;
      if (control != null)
        control.OnButtonMarginChanged();
    }

    public bool IsPin
    {
      get { return (Boolean)GetValue(IsPinProperty); }
      set { SetValue(IsPinProperty, value); }
    }
    public static readonly DependencyProperty IsPinProperty = DependencyProperty.Register("IsPin", typeof(bool), typeof(CalculatorKeyboard), new PropertyMetadata(false));

    public bool ResetOnCalculation
    {
      get { return (Boolean)GetValue(ResetOnCalculationProperty); }
      set { SetValue(ResetOnCalculationProperty, value); }
    }
    public static readonly DependencyProperty ResetOnCalculationProperty = DependencyProperty.Register("ResetOnCalculation", typeof(bool), typeof(CalculatorKeyboard), new PropertyMetadata(true, new PropertyChangedCallback(OnResetOnCalculationChanged)));
    private static void OnResetOnCalculationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var control = d as CalculatorKeyboard;
      if (control != null)
        control.OnResetOnCalculationChanged();
    }

    #endregion

    #region Variables
    private Int32 _buttonFontSize = 16;
    private Double _displayScreenWidth = 50;

    #endregion
  }
}

using System;
using System.ComponentModel;
using System.Windows;
using Hexagon.Software.NCGage.HelperLib;

namespace Hexagon.Software.NCGage.UserControls
{
  public partial class CalculatorKeyboardControl : KeyboardControl
  {
    public CalculatorKeyboardControl()
    {
      InitializeComponent();
    }

    private void UpdateDisplayScreenSize()
    {
      var columns = this.ViewModel.KeyboardType == KeyboardTypes.Calculator ? 9 : 4;
      this.DisplayScreenWidth = this.ButtonSize * columns + this.ButtonMargin * 2 * (columns - 1);
    }

    protected void btnEnter_Click(object sender, RoutedEventArgs e)
    {
      if (this.InputTarget != null)
      {
        this.InputTarget.Clear();
        Helpers.RaiseTextInputEvent(this.InputTarget, this.ViewModel.InputValue);
      }
    }

    protected override void OnInitialized(object sender, EventArgs e)
    {
      this.ViewModel = this.DataContext as CalculatorKeyboardControlViewModel;
      this.ViewModel.PropertyChanged += ViewModel_PropertyChanged;
      this.OnResetOnCalculationChanged();

      base.OnInitialized(sender, e);
    }
    protected override void OnStartupKeyboardTypeChanged()
    {
      this.ViewModel.KeyboardType = this.StartupKeyboardType;
    }
    protected override void OnButtonSizeChanged()
    {
      this.UpdateDisplayScreenSize();
      base.OnButtonSizeChanged();
    }
    protected override void OnButtonMarginChanged()
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


    #region Properties
    protected CalculatorKeyboardControlViewModel ViewModel { get; private set; }
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
    public bool ResetOnCalculation
    {
      get { return (Boolean)GetValue(ResetOnCalculationProperty); }
      set { SetValue(ResetOnCalculationProperty, value); }
    }
    public static readonly DependencyProperty ResetOnCalculationProperty = DependencyProperty.Register("ResetOnCalculation", typeof(bool), typeof(CalculatorKeyboardControl), new PropertyMetadata(true, OnResetOnCalculationChanged));
    private static void OnResetOnCalculationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var control = d as CalculatorKeyboardControl;
      if (control != null)
        control.OnResetOnCalculationChanged();
    }

    #endregion

    #region Variables
    private Double _displayScreenWidth = 50;

    #endregion
  }
}

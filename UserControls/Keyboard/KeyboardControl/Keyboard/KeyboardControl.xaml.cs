using Hexagon.Software.NCGage.HelperLib;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Hexagon.Software.NCGage.UserControls
{
  public partial class KeyboardControl : KeyboardControlBase
  {
    public KeyboardControl()
    {
      InitializeComponent();
      this.ViewModel = this.DataContext as KeyboardControlViewModel;
      this.ViewModel.RefreshKeyboard(StartupKeyboardType);

      this.MouseRightButtonDown += KeyboardControl_MouseRightButtonDown;
    }

    private void KeyboardControl_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
    {
      if (this.StartupKeyboardType == KeyboardTypes.Full)
        this.StartupKeyboardType = KeyboardTypes.Number;
      else if (this.StartupKeyboardType == KeyboardTypes.Number)
        this.StartupKeyboardType = KeyboardTypes.Calculator;
      else
        this.StartupKeyboardType = KeyboardTypes.Full;
    }

    protected override void OnStartupKeyboardTypeChanged()
    {
      this.ViewModel?.RefreshKeyboard(StartupKeyboardType);
    }

    #region Dependency Properties
    public bool ResetOnCalculation
    {
      get { return (Boolean)GetValue(ResetOnCalculationProperty); }
      set { SetValue(ResetOnCalculationProperty, value); }
    }
    public static readonly DependencyProperty ResetOnCalculationProperty = DependencyProperty.Register("ResetOnCalculation", typeof(bool), typeof(KeyboardControl), new PropertyMetadata(true));

    #endregion

    public KeyboardControlViewModel ViewModel { get; private set; }
  }
}

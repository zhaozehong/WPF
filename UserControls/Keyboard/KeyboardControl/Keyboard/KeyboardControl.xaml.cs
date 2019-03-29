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
    }

    private void KeyboardControl_Closed(object sender, EventArgs e)
    {
      base.FireClosedEvent();
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

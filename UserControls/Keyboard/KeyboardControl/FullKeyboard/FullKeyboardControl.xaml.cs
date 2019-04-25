using Hexagon.Software.NCGage.HelperLib;
using System;
using System.Windows;

namespace Hexagon.Software.NCGage.UserControls
{
  public partial class FullKeyboardControl : KeyboardControlBase
  {
    public FullKeyboardControl()
    {
      InitializeComponent();
    }

    protected void btnClose_Click(object sender, RoutedEventArgs e)
    {
      base.FireClosedEvent();
    }
    protected void btnPin_Click(object sender, RoutedEventArgs e)
    {
      this.IsPin = !IsPin;
    }
    private void btnSymbol_click(object sender, RoutedEventArgs e)
    {
      var vm = this.DataContext as FullKeyboardControlViewModel;
      if (vm != null)
        vm.DisplayMode = vm.DisplayMode == DisplayModes.Symbol ? DisplayModes.Normal : DisplayModes.Symbol;
    }
    private void btnCapital_click(object sender, RoutedEventArgs e)
    {
      var vm = this.DataContext as FullKeyboardControlViewModel;
      if (vm != null)
        vm.DisplayMode = vm.DisplayMode == DisplayModes.Capital ? DisplayModes.Normal : DisplayModes.Capital;
    }

    private void btnJapanese_click(object sender, RoutedEventArgs e)
    {
      var vm = this.DataContext as FullKeyboardControlViewModel;
      if (vm != null)
      {
        //if(vm.DisplayMode != DisplayModes.Japanese)
        //{
        //  _previousMode = vm.DisplayMode;
        //  vm.DisplayMode = DisplayModes.Japanese;
        //}
        //else
        //{
        //  vm.DisplayMode = _previousMode;
        //}

        vm.DisplayMode = DisplayModes.Japanese;
      }
    }
  }
}

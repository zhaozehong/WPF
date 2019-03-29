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

      // ZEHONG: just a workaround for fixing a bug: the first time to input something, it always go to catch
      this.Loaded += (s, e) =>
      {
        try
        {
          FullKeyAction action;
          FullKeyboardHelper.CharActionPairs.TryGetValue("q", out action);
        }
        catch (Exception) { }
      };
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
  }
}

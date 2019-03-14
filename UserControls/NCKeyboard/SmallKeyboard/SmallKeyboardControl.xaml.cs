using System;
using System.Windows;
using System.Windows.Controls;
using WPF.Helper;

namespace WPF.UserControls
{
  public partial class SmallKeyboardControl : KeyboardControl
  {
    public SmallKeyboardControl()
    {
      InitializeComponent();

      var random = new Random();
      this.MouseRightButtonUp += (s, e) =>
      {
        var value = random.Next(30, 100);
        this.ButtonSize = value;
      };
    }


    #region Dependency Properties
    public Double ButtonSize
    {
      get { return (Double)GetValue(ButtonSizeProperty); }
      set { SetValue(ButtonSizeProperty, value); }
    }
    public static readonly DependencyProperty ButtonSizeProperty =
        DependencyProperty.Register("ButtonSize", typeof(Double), typeof(SmallKeyboardControl), new PropertyMetadata(60.0, new PropertyChangedCallback(OnButtonSizeChangeddddd)));

    private static void OnButtonSizeChangeddddd(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
    }

    #endregion

    private void KeyboardControl_Switched(object sender, EventArgs e)
    {
      var vm = this.DataContext as SmallKeyboardControlViewModel;
      if (vm != null)
        vm.SwitchKeyboard();
    }
    private void KeyboardControl_Closed(object sender, EventArgs e)
    {
      this.FireClosedEvent(sender, e);
    }
  }

  public class SmallKeyboardControlViewModel : NotifyPropertyChanged
  {
    public SmallKeyboardControlViewModel()
    {
      this.CurrentViewModel = _numberKeyboardViewModel;
    }
    public void SwitchKeyboard()
    {
      this.CurrentViewModel = Object.ReferenceEquals(CurrentViewModel, _numberKeyboardViewModel) ? _calculatorKeyboardViewModel : _numberKeyboardViewModel;
    }

    public KeyboardViewModelBase CurrentViewModel
    {
      get { return _currentViewModel; }
      set
      {
        if (_currentViewModel != value)
        {
          _currentViewModel = value;
          this.RaisePropertyChanged(nameof(CurrentViewModel));
        }
      }
    }

    private KeyboardViewModelBase _numberKeyboardViewModel = new NumberKeyboardControlViewModel();
    private KeyboardViewModelBase _calculatorKeyboardViewModel = new CalculatorKeyboardControlViewModel();
    private KeyboardViewModelBase _currentViewModel;
  }
}

using Hexagon.Software.NCGage.HelperLib;

namespace Hexagon.Software.NCGage.UserControls
{
  public class KeyboardControlViewModel : NotifyPropertyChanged
  {
    public KeyboardControlViewModel()
    {
      CurrentViewModel = _calculatorKeyboardControlVM;
    }

    public void RefreshKeyboard(KeyboardTypes keyboardType)
    {
      this.CurrentViewModel = keyboardType == KeyboardTypes.Full ? _fullKeyboardControlVM : _calculatorKeyboardControlVM;
    }

    public KeyboardControlViewModelBase CurrentViewModel
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

    private KeyboardControlViewModelBase _calculatorKeyboardControlVM = new CalculatorKeyboardControlViewModel();
    private KeyboardControlViewModelBase _fullKeyboardControlVM = new FullKeyboardControlViewModel();
    private KeyboardControlViewModelBase _currentViewModel;
  }
}

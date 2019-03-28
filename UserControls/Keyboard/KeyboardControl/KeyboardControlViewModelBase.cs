using Hexagon.Software.NCGage.HelperLib;

namespace Hexagon.Software.NCGage.UserControls
{
  public abstract class KeyboardControlViewModelBase : NotifyPropertyChanged
  {
    public KeyboardControlViewModelBase()
    {
      this.ButtonCommand = new RelayCommand(Update);
    }
    protected abstract void Update(object parameter);

    public RelayCommand ButtonCommand { get; private set; }
  }
}

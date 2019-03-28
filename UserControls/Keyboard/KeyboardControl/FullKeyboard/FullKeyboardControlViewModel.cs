using Hexagon.Software.NCGage.HelperLib;

namespace Hexagon.Software.NCGage.UserControls
{
  public class FullKeyboardControlViewModel : KeyboardControlViewModelBase
  {
    protected override void Update(object parameter)
    {
      var button = parameter as FullKeyboardButton;
      if (button != null)
        button.SendKey();
    }

    public DisplayModes DisplayMode
    {
      get { return _displayMode; }
      set
      {
        if (_displayMode != value)
        {
          _displayMode = value;
          this.RaisePropertyChanged(nameof(DisplayMode));
        }
      }
    }

    public DisplayModes _displayMode = DisplayModes.Normal;
  }
}

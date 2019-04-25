using Hexagon.Software.NCGage.HelperLib;

namespace Hexagon.Software.NCGage.UserControls
{
  public class FullKeyboardControlViewModel : KeyboardControlViewModelBase
  {
    protected override void Update(object parameter)
    {
      var button = parameter as FullKeyboardButton;
      if (button != null)
        button.Fire();
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
          this.RaisePropertyChanged(nameof(IsFullJapanese));
          Helpers.SetInputMethod(_displayMode, _isFull);
        }
        else if (value == DisplayModes.Japanese)
        {
          this.IsFull = !this.IsFull;
        }
      }
    }
    public bool IsFull
    {
      get { return _isFull; }
      private set
      {
        if (_isFull != value)
        {
          _isFull = value;
          this.RaisePropertyChanged(nameof(IsFull));
          this.RaisePropertyChanged(nameof(IsFullJapanese));

          Helpers.SetInputMethod(_displayMode, _isFull);
        }
      }
    }
    public bool IsFullJapanese
    {
      get { return this.IsFull && this.DisplayMode == DisplayModes.Japanese; }
    }

    private DisplayModes _displayMode = DisplayModes.Normal;
    private bool _isFull  = true;
  }
}

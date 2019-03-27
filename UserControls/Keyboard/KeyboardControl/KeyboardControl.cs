using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Hexagon.Software.NCGage.UserControls
{
  public abstract class KeyboardControl : UserControl, INotifyPropertyChanged
  {
    public KeyboardControl()
    {
      this.Loaded += OnLoaded;
    }

    protected void btnClose_Click(object sender, RoutedEventArgs e)
    {
      if (Closed != null)
        Closed(this, null);
    }
    protected void btnPin_Click(object sender, RoutedEventArgs e)
    {
      this.IsPin = !IsPin;
    }
    protected void btnCLR_Click(object sender, RoutedEventArgs e)
    {
      if (this.InputTarget != null)
        this.InputTarget.Clear();
    }

    protected virtual void OnLoaded(object sender, RoutedEventArgs e)
    {
#if !DEBUG
         this.IsPin = false;
#endif

      this.OnStartupKeyboardTypeChanged();
      this.OnButtonSizeChanged();
    }
    protected virtual void OnStartupKeyboardTypeChanged() { }
    protected virtual void OnButtonSizeChanged()
    {
      this.UpdateButtonFontSize();
    }
    protected virtual void OnButtonMarginChanged() { }

    private void UpdateButtonFontSize()
    {
      this.ButtonFontSize = (Int32)(this.ButtonSize / 2.2);
    }

    #region INotifyPropertyChanged
    public event PropertyChangedEventHandler PropertyChanged;
    protected void RaisePropertyChanged(String propertyName)
    {
      PropertyChangedEventHandler handler = null;
      lock (this)
      {
        handler = PropertyChanged;
      }
      if (handler != null)
        handler(this, new PropertyChangedEventArgs(propertyName));
    }
    #endregion

    public event EventHandler Closed;

    private Int32 _buttonFontSize = 16;
    public Int32 ButtonFontSize
    {
      get { return _buttonFontSize; }
      set
      {
        if (_buttonFontSize != value)
        {
          _buttonFontSize = value;
          this.RaisePropertyChanged(nameof(ButtonFontSize));
        }
      }
    }

    #region Dependency Properties
    public TextBox InputTarget
    {
      get { return (TextBox)GetValue(InputTargetProperty); }
      set { SetValue(InputTargetProperty, value); }
    }
    public static readonly DependencyProperty InputTargetProperty = DependencyProperty.Register("InputTarget", typeof(TextBox), typeof(KeyboardControl), new PropertyMetadata(null));

    public KeyboardTypes StartupKeyboardType
    {
      get { return (KeyboardTypes)GetValue(StartupKeyboardTypeProperty); }
      set { SetValue(StartupKeyboardTypeProperty, value); }
    }
    public static readonly DependencyProperty StartupKeyboardTypeProperty = DependencyProperty.Register("StartupKeyboardType", typeof(KeyboardTypes), typeof(KeyboardControl), new PropertyMetadata(KeyboardTypes.Number, new PropertyChangedCallback(OnStartupKeyboardTypeChanged)));
    private static void OnStartupKeyboardTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var control = d as KeyboardControl;
      if (control != null)
        control.OnStartupKeyboardTypeChanged();
    }

    public double ButtonSize
    {
      get { return (Double)GetValue(ButtonSizeProperty); }
      set { SetValue(ButtonSizeProperty, value); }
    }
    public static readonly DependencyProperty ButtonSizeProperty = DependencyProperty.Register("ButtonSize", typeof(double), typeof(KeyboardControl), new PropertyMetadata(60.0, new PropertyChangedCallback(OnButtonSizeChanged)));
    private static void OnButtonSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var control = d as KeyboardControl;
      if (control != null)
        control.OnButtonSizeChanged();
    }

    public double ButtonMargin
    {
      get { return (double)GetValue(ButtonMarginProperty); }
      set { SetValue(ButtonMarginProperty, value); }
    }
    public static readonly DependencyProperty ButtonMarginProperty = DependencyProperty.Register("ButtonMargin", typeof(double), typeof(KeyboardControl), new PropertyMetadata(1.0, new PropertyChangedCallback(OnButtonMarginChanged)));
    private static void OnButtonMarginChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var control = d as KeyboardControl;
      if (control != null)
        control.OnButtonMarginChanged();
    }

    public bool IsPin
    {
      get { return (Boolean)GetValue(IsPinProperty); }
      set { SetValue(IsPinProperty, value); }
    }
    public static readonly DependencyProperty IsPinProperty = DependencyProperty.Register("IsPin", typeof(bool), typeof(KeyboardControl), new PropertyMetadata(false));

    #endregion
  }
}

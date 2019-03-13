using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WPF.UserControls
{
  public class KeyboardButton : ContentControl
  {
    static KeyboardButton()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(KeyboardButton), new FrameworkPropertyMetadata(typeof(KeyboardButton)));
    }

    public void Handle(StringBuilder sbSource)
    {
      switch (this.Key)
      {
        case KeyboardKeys.D0:
        case KeyboardKeys.D1:
        case KeyboardKeys.D2:
        case KeyboardKeys.D3:
        case KeyboardKeys.D4:
        case KeyboardKeys.D5:
        case KeyboardKeys.D6:
        case KeyboardKeys.D7:
        case KeyboardKeys.D8:
        case KeyboardKeys.D9:
          sbSource.Append(this.Key.ToString().Last());
          break;
        case KeyboardKeys.Backspace:
          sbSource.Remove(sbSource.Length - 1, 1);
          break;
        case KeyboardKeys.Minus:
          sbSource.Append("-");
          break;
        case KeyboardKeys.Point:
          sbSource.Append(".");
          break;
        case KeyboardKeys.Enter:
        case KeyboardKeys.Pin:
        case KeyboardKeys.CLR:
          break;
      }
    }
    protected virtual void OnCommandChanged(ICommand oldCommand, ICommand newCommand)
    {
      // Unhook old command
      if (oldCommand != null)
        oldCommand.CanExecuteChanged -= CanExecuteChangedHandler;

      // Hookup new command
      _canExecuteChangedHandler = new EventHandler(CanExecuteChangedHandler);
      if (newCommand != null)
        newCommand.CanExecuteChanged += _canExecuteChangedHandler;

      CanExecuteChangedHandler(null, null);
    }
    protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
    {
      base.OnMouseLeftButtonDown(e);
      this.IsPressed = true;
    }
    protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
    {
      try
      {
        base.OnMouseLeftButtonUp(e);

        this.RaiseEvent(new RoutedEventArgs(ClickEvent));
        this.RaiseCommand();
      }
      catch (Exception) { }
      finally
      {
        this.IsPressed = false;
      }
    }

    private void CanExecuteChangedHandler(object sender, EventArgs e)
    {
      if (this.Command == null)
        return;

      var rc = Command as RoutedCommand;
      if (rc != null)
        IsEnabled = rc.CanExecute(CommandParameter, CommandTarget);
      else
        IsEnabled = Command.CanExecute(CommandParameter);
    }
    private void RaiseCommand()
    {
      if (Command == null)
        return;

      var rc = Command as RoutedCommand;
      if (rc != null)
        rc.Execute(CommandParameter, CommandTarget);
      else
        Command.Execute(CommandParameter);
    }

    #region Events
    private static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent("Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(KeyboardButton));
    public event RoutedEventHandler Click
    {
      add { AddHandler(ClickEvent, value); }
      remove { RemoveHandler(ClickEvent, value); }
    }

    #endregion

    #region Dependency Properties
    public CornerRadius CornerRadius
    {
      get { return (CornerRadius)GetValue(CornerRadiusProperty); }
      set { SetValue(CornerRadiusProperty, value); }
    }
    public bool IsPressed
    {
      get { return (Boolean)GetValue(IsPressedProperty); }
      set { SetValue(IsPressedProperty, value); }
    }
    public KeyboardKeys Key
    {
      get { return (KeyboardKeys)GetValue(KeyboardKeyProperty); }
      set { SetValue(KeyboardKeyProperty, value); }
    }
    public ICommand Command
    {
      get { return (ICommand)GetValue(CommandProperty); }
      set { SetValue(CommandProperty, value); }
    }
    public object CommandParameter
    {
      get { return (object)GetValue(CommandParameterProperty); }
      set { SetValue(CommandParameterProperty, value); }
    }
    public IInputElement CommandTarget
    {
      get { return (IInputElement)GetValue(CommandTargetProperty); }
      set { SetValue(CommandTargetProperty, value); }
    }

    public static readonly DependencyProperty CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(KeyboardButton), new PropertyMetadata(new CornerRadius(10)));
    public static readonly DependencyProperty IsPressedProperty = DependencyProperty.Register("IsPressed", typeof(Boolean), typeof(KeyboardButton));
    public static readonly DependencyProperty KeyboardKeyProperty = DependencyProperty.Register("KeyboardKey", typeof(KeyboardKeys), typeof(KeyboardButton), new PropertyMetadata(KeyboardKeys.None));
    private static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(KeyboardButton),
      new PropertyMetadata((ICommand)null, new PropertyChangedCallback(OnCommandChanged)));
    private static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register("CommandParameter", typeof(object), typeof(KeyboardButton), new PropertyMetadata(null));
    private static readonly DependencyProperty CommandTargetProperty = DependencyProperty.Register("CommandTarget", typeof(IInputElement), typeof(KeyboardButton), new PropertyMetadata(null));

    private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var control = d as KeyboardButton;
      if (control != null)
        control.OnCommandChanged((ICommand)e.OldValue, (ICommand)e.NewValue);
    }

    #endregion

    private EventHandler _canExecuteChangedHandler; // keeps a copy of the CanExecuteChanged handler so it doesn't get garbage collected
  }
}

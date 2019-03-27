using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Hexagon.Software.NCGage.UserControls
{
  public abstract class KeyboardButton : ContentControl, ICommandSource
  {
    static KeyboardButton()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(KeyboardButton), new FrameworkPropertyMetadata(typeof(KeyboardButton)));
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

        // Zehong: run RaiseCommand before RaiseEvent, cos RaiseCommand will auto-fill bracket
        this.RaiseCommand();
        this.RaiseEvent(new RoutedEventArgs(ClickEvent));
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
    private static readonly RoutedEvent ClickEvent = EventManager.RegisterRoutedEvent("Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(CalculatorKeyboardButton));
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

    private EventHandler _canExecuteChangedHandler; // keeps a copy of the CanExecuteChanged handler so it doesn't get garbage collected55600
  }
}

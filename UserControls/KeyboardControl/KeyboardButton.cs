using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Hexagon.Software.NCGage.HelperLib;

namespace Hexagon.Software.NCGage.UserControls
{
  public class KeyboardButton : ContentControl, ICommandSource
  {
    static KeyboardButton()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(KeyboardButton), new FrameworkPropertyMetadata(typeof(KeyboardButton)));
    }

    public Boolean Handle(List<InputInfo> recordList, string preValue)
    {
      if (recordList == null)
        return false;

      String value = null;
      switch (this.Key)
      {
        case KeyboardKeys.D_D0:
        case KeyboardKeys.D_D1:
        case KeyboardKeys.D_D2:
        case KeyboardKeys.D_D3:
        case KeyboardKeys.D_D4:
        case KeyboardKeys.D_D5:
        case KeyboardKeys.D_D6:
        case KeyboardKeys.D_D7:
        case KeyboardKeys.D_D8:
        case KeyboardKeys.D_D9:
          value = this.Key.ToString().Last().ToString();
          break;

        case KeyboardKeys.Point:
          value = ".";
          break;

        case KeyboardKeys.O_Add:
          value = "+";
          break;
        case KeyboardKeys.O_Substract:
          value = "-";
          break;
        case KeyboardKeys.O_Multiply:
          value = "*";
          break;
        case KeyboardKeys.O_Divide:
          value = "/";
          break;
        case KeyboardKeys.O_Mod:
          value = "%";
          break;
        case KeyboardKeys.O_Square:
          value = "^";
          break;

        case KeyboardKeys.PI:
          value = String.Format("{0:F4}", Math.PI);
          break;

        // functions
        case KeyboardKeys.F_Abs:
        case KeyboardKeys.F_Sin:
        case KeyboardKeys.F_Cos:
        case KeyboardKeys.F_Tan:
        case KeyboardKeys.F_In:
        case KeyboardKeys.F_Sqrt:
        case KeyboardKeys.F_ASin:
        case KeyboardKeys.F_ACos:
        case KeyboardKeys.F_ATan:
        case KeyboardKeys.F_Exp:
          value = String.Format("{0}(", Key.ToString().Substring(2));
          break;
        case KeyboardKeys.LeftBracket:
          value = "(";
          break;
        case KeyboardKeys.RightBracket:
          value = ")";
          break;

        case KeyboardKeys.Backspace:
        case KeyboardKeys.C:
          var lastValue = recordList.LastOrDefault();
          if (lastValue != null)
          {
            if (lastValue.Value == "2" && lastValue.Previous != null && lastValue.Previous.Value == "^")
              recordList.RemoveAt(recordList.Count - 1);
            recordList.RemoveAt(recordList.Count - 1);
          }
          break;
        case KeyboardKeys.AC:
          recordList.Clear();
          break;

        case KeyboardKeys.None:
        case KeyboardKeys.Enter:
        case KeyboardKeys.Equal:
        case KeyboardKeys.SWITCH:
        case KeyboardKeys.Pin:
        case KeyboardKeys.CLR:
        case KeyboardKeys.Inv:
        case KeyboardKeys.M2I:
        case KeyboardKeys.I2M:
        case KeyboardKeys.Close:
          break;
      }
      value = String.Format("{0}{1}", preValue, value);
      if (!String.IsNullOrWhiteSpace(value))
        recordList.Add(new InputInfo(recordList.LastOrDefault(), this.Key, value));
      if (this.Key == KeyboardKeys.O_Square)
        recordList.Add(new InputInfo(recordList.LastOrDefault(), KeyboardKeys.D_D2, "2"));

      return true;
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

    private EventHandler _canExecuteChangedHandler; // keeps a copy of the CanExecuteChanged handler so it doesn't get garbage collected55600
  }
  public class InputInfo
  {
    public InputInfo(InputInfo previous, KeyboardKeys key, String value)
    {
      this.Previous = previous;
      this.Key = key;
      this.Value = value;

      this.IsFunction = KeyboardHelper.IsFunctionKey(this.Key);
      this.IsOperator = KeyboardHelper.IsOperatorKey(this.Key);
      this.IsDigit = KeyboardHelper.IsDigitKey(this.Key);
      this.IsSubmit = KeyboardHelper.IsSubmitKey(this.Key);
      this.IsPoint = KeyboardHelper.IsPointKey(this.Key);
      this.IsLeftBracket = KeyboardHelper.IsLeftBracketKey(this.Key);
      this.IsRightBracket = KeyboardHelper.IsRightBracketKey(this.Key);
    }

    public InputInfo Previous { get; private set; }
    public KeyboardKeys Key { get; private set; }
    public String Value { get; private set; }

    public Boolean IsFunction { get; private set; }
    public Boolean IsOperator { get; private set; }
    public Boolean IsDigit { get; private set; }
    public Boolean IsSubmit { get; private set; }
    public Boolean IsPoint { get; private set; }
    public Boolean IsLeftBracket { get; private set; }
    public Boolean IsRightBracket { get; private set; }
  }
}

﻿using Hexagon.Software.NCGage.HelperLib;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Hexagon.Software.NCGage.UserControls
{
  public abstract class KeyboardControlBase : UserControl, INotifyPropertyChanged
  {
    public KeyboardControlBase()
    {
      this.Initialized += OnInitialized;
    }

    protected void FireClosedEvent()
    {
      if (Closed != null)
        Closed(this, null);
    }

    protected virtual void OnInitialized(object sender, EventArgs e)
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
      this.ButtonFontSize = (Int32)(this.ButtonSize / 2.5);
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
    public static readonly DependencyProperty InputTargetProperty = DependencyProperty.Register("InputTarget", typeof(TextBox), typeof(KeyboardControlBase), new PropertyMetadata(null));

    public KeyboardTypes StartupKeyboardType
    {
      get { return (KeyboardTypes)GetValue(StartupKeyboardTypeProperty); }
      set { SetValue(StartupKeyboardTypeProperty, value); }
    }
    public static readonly DependencyProperty StartupKeyboardTypeProperty = DependencyProperty.Register("StartupKeyboardType", typeof(KeyboardTypes), typeof(KeyboardControlBase), new PropertyMetadata(KeyboardTypes.Number, OnStartupKeyboardTypeChanged));
    private static void OnStartupKeyboardTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var control = d as KeyboardControlBase;
      if (control != null)
        control.OnStartupKeyboardTypeChanged();
    }

    public double ButtonSize
    {
      get { return (Double)GetValue(ButtonSizeProperty); }
      set { SetValue(ButtonSizeProperty, value); }
    }
    public static readonly DependencyProperty ButtonSizeProperty = DependencyProperty.Register("ButtonSize", typeof(double), typeof(KeyboardControlBase), new PropertyMetadata(60.0, OnButtonSizeChanged));
    private static void OnButtonSizeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var control = d as KeyboardControlBase;
      if (control != null)
        control.OnButtonSizeChanged();
    }

    public double ButtonMargin
    {
      get { return (double)GetValue(ButtonMarginProperty); }
      set { SetValue(ButtonMarginProperty, value); }
    }
    public static readonly DependencyProperty ButtonMarginProperty = DependencyProperty.Register("ButtonMargin", typeof(double), typeof(KeyboardControlBase), new PropertyMetadata(1.0, OnButtonMarginChanged));
    private static void OnButtonMarginChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var control = d as KeyboardControlBase;
      if (control != null)
        control.OnButtonMarginChanged();
    }

    public bool IsPin
    {
      get { return (Boolean)GetValue(IsPinProperty); }
      set { SetValue(IsPinProperty, value); }
    }
    public static readonly DependencyProperty IsPinProperty = DependencyProperty.Register("IsPin", typeof(bool), typeof(KeyboardControlBase), new PropertyMetadata(false));

    #endregion
  }
}

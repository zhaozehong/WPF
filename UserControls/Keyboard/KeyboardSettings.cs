﻿using System;
using System.Windows.Media;
using Hexagon.Software.NCGage.HelperLib;

namespace Hexagon.Software.NCGage.UserControls
{
  public class KeyboardSettings : NotifyPropertyChanged
  {
    private Boolean _enableKeyboard = false;
    public Boolean EnableKeyboard
    {
      get { return _enableKeyboard; }
      set
      {
        if (_enableKeyboard != value)
        {
          _enableKeyboard = value;
          this.RaisePropertyChanged(() => EnableKeyboard);
        }
      }
    }

    private SolidColorBrush _fontBrush = Brushes.White;
    public SolidColorBrush FontBrush
    {
      get { return _fontBrush; }
      set
      {
        if (!object.Equals(_fontBrush, value))
        {
          _fontBrush = value;
          this.RaisePropertyChanged(() => FontBrush);
        }
      }
    }


    private static String LockObj = String.Empty;
    private static KeyboardSettings _current;
    public static KeyboardSettings Current
    {
      get
      {
        lock(LockObj)
        {
          if (_current == null)
            _current = new KeyboardSettings();
          return _current;
        }
      }
    }
  }
}

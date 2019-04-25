using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Linq;
using System.Windows.Input;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Windows.Interop;
using System.Globalization;

namespace Hexagon.Software.NCGage.HelperLib
{
  public static class Helpers
  {
    public static Double MIN_VALUE = 1e-8;
    public static Point ZeroPoint = new Point(0, 0);
    public static bool IsRuningAsServiceMode { get { return false; } }

    public static Boolean IsNullOrZero(Double? value)
    {
      return value == null || Double.IsNaN(value.Value) || Math.Abs(value.Value) < MIN_VALUE;
    }
    public static FormattedText MeasureText(TextBox textBox, double fontSize)
    {
      return new FormattedText(textBox.Text, System.Threading.Thread.CurrentThread.CurrentUICulture, FlowDirection.LeftToRight,
        new Typeface(textBox.FontFamily, textBox.FontStyle, textBox.FontWeight, textBox.FontStretch),
        fontSize, textBox.Foreground);
    }

    public static double CalculateMaxWidthFontSize(TextBox textBox)
    {
      var boxWidth = Double.IsNaN(textBox.Width) ? textBox.ActualWidth : textBox.Width;
      if (double.IsNaN(boxWidth))
        return textBox.FontSize;

      double endSize = textBox.FontSize;
      if (MeasureText(textBox, endSize).WidthIncludingTrailingWhitespace < boxWidth)
        return endSize;

      double startSize = 0, space = 1;
      while (endSize > 0)
      {
        var middleSize = (endSize + startSize) / 2;
        var textWidth = MeasureText(textBox, middleSize).WidthIncludingTrailingWhitespace;
        if (IsNullOrZero(textWidth - boxWidth))
        {
          return middleSize;
        }
        else if (middleSize < boxWidth)
        {
          if (MeasureText(textBox, middleSize + space).WidthIncludingTrailingWhitespace > boxWidth)
            return middleSize;
          startSize = middleSize + space;
        }
        else
        {
          endSize = middleSize - space;
        }
      }
      return endSize;
    }
    public static String ListToValue<T>(List<T> sourceList, String separator = "")
    {
      if (sourceList == null || !sourceList.Any())
        return String.Empty;

      var sb = new StringBuilder();
      if (!String.IsNullOrEmpty(separator))
      {
        sourceList.ForEach(p => sb.Append(String.Format("{0}{1}", p.ToString(), separator)));
        sb.Remove(sb.Length - 1, 1);
      }
      else
      {
        sourceList.ForEach(p => sb.Append(p.ToString()));
      }
      return sb.ToString();
    }

    public static void RaiseTextInputEvent(UIElement element, string text)
    {
      if (element == null)
        return;

      InputManager inputManager = InputManager.Current;
      InputDevice inputDevice = inputManager.PrimaryKeyboardDevice;
      TextComposition composition = new TextComposition(inputManager, element, text);
      TextCompositionEventArgs args = new TextCompositionEventArgs(inputDevice, composition);
      args.RoutedEvent = UIElement.PreviewTextInputEvent;
      element.RaiseEvent(args);
      args.RoutedEvent = UIElement.TextInputEvent;
      element.RaiseEvent(args);
    }
    public static double GetWindowLeft(Window window)
    {
      if (window.WindowState == WindowState.Maximized)
      {
        var leftField = typeof(Window).GetField("_actualLeft", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        return (double)leftField.GetValue(window);
      }
      else
      {
        return window.Left;
      }
    }
    public static int DIUToPixels(double value, bool isWidth = true)
    {
      if (isWidth)
        return (int)(value * System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width / SystemParameters.WorkArea.Width);
      else
        return (int)(value * System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height / SystemParameters.WorkArea.Height);
    }
    public static double PixelsToDIU(int pixels, bool isWidth = true)
    {
      if (isWidth)
        return pixels * SystemParameters.WorkArea.Width / System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width;
      else
        return pixels * SystemParameters.WorkArea.Height / System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height;
    }
    public static Size GetElementPixelSize(UIElement element)
    {
      Matrix transformToDevice;
      var source = PresentationSource.FromVisual(element);
      if (source != null)
        transformToDevice = source.CompositionTarget.TransformToDevice;
      else
        using (var source1 = new System.Windows.Interop.HwndSource(new System.Windows.Interop.HwndSourceParameters()))
          transformToDevice = source1.CompositionTarget.TransformToDevice;

      if (element.DesiredSize == new Size())
        element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));

      return (Size)transformToDevice.Transform((Vector)element.DesiredSize);
    }

    public static int GetCount(String strSource, String pattern)
    {
      return String.IsNullOrWhiteSpace(strSource) ? 0 : Regex.Matches(strSource, pattern).OfType<Match>().Count();
    }
    public static Boolean IsNumericValue(String strValue)
    {
      return Regex.IsMatch(strValue, @"^-?\d+\.?\d*$");
    }
    public static String GetSubExpression(String strExpression, bool checkCount = true)
    {
      if (String.IsNullOrWhiteSpace(strExpression))
        return null;

      var leftIndexs = Regex.Matches(strExpression, @"[([{]").OfType<Match>().Select(m => m.Index).ToList();
      var rightIndexs = Regex.Matches(strExpression, @"[)\]}]").OfType<Match>().Select(m => m.Index).ToList();
      if (checkCount && leftIndexs.Count != rightIndexs.Count)
        return null;

      var stopIndexs = rightIndexs.Where((r, index) => leftIndexs.Count(l => l < r) == (index + 1));
      return stopIndexs.Any() ? strExpression.Substring(0, stopIndexs.FirstOrDefault() + 1) : strExpression;
    }
    public static String GetLastSubExpression(String strExpression)
    {
      var index = GetLastSubExpressionIndex(strExpression);
      return index != -1 ? strExpression.Substring(index) : null;
    }
    public static Int32 GetLastSubExpressionIndex(String strExpression)
    {
      if (String.IsNullOrWhiteSpace(strExpression))
        return -1;

      var leftIndexs = Regex.Matches(strExpression, @"[([{]").OfType<Match>().Select(m => m.Index).ToList();
      var rightIndexs = Regex.Matches(strExpression, @"[)\]}]").OfType<Match>().Select(m => m.Index).ToList();
      if (!leftIndexs.Any() || !rightIndexs.Any())
        return -1;

      var iEnd = rightIndexs.LastOrDefault();
      var stopIndexs = leftIndexs.Where((i) => rightIndexs.Count(r => r >= i && r <= iEnd) == leftIndexs.Count(l => l >= i && l <= iEnd));
      return stopIndexs.Any() ? stopIndexs.FirstOrDefault() : -1;
    }
    public static String GetValueString(Nullable<Double> value, Int32 decimals = 4)
    {
      if (value == null || Double.IsNaN(value.Value))
        return String.Empty;
      return String.Format("{0:F" + decimals.ToString() + "}", value);
    }

    public static bool IsBrowserApplication { get { return System.Windows.Interop.BrowserInteropHelper.IsBrowserHosted; } }
    public static bool? ShowDialog(Window dlg)
    {
      return ShowDialog(dlg, true);
    }
    public static bool? ShowDialog(Window dlg, Boolean isDefaultPosition)
    {
      if (Application.Current != null && Application.Current.CheckAccess() == false)
      {
        bool? ret = null;
        Application.Current.Dispatcher.Invoke(new Action(() => ret = ShowDialog(dlg, isDefaultPosition)));
        return ret;
      }

      Debug.Assert(dlg != null);
      if (dlg == null)
        return null;
      bool? isEnabledOld = null;
      bool isTopmostOld = dlg.Topmost;
      if (IsBrowserApplication)
      {
        if (Application.Current != null && Application.Current.MainWindow != null)
        {
          isEnabledOld = OS.Win32.IsWindowEnabled(OS.Win32.GetActiveWindow());
          OS.Win32.EnableWindow(OS.Win32.GetActiveWindow(), false);

          if (isDefaultPosition)
            dlg.Loaded += new RoutedEventHandler(OnBrowserApplicationDialog_Loaded);
        }
        dlg.Topmost = true;
      }
      else
      {
        if (Application.Current != null && !dlg.Equals(Application.Current.MainWindow) && Application.Current.MainWindow.IsLoaded
          && Application.Current.MainWindow.Visibility == Visibility.Visible)
        {
          dlg.Owner = Application.Current.MainWindow;
          if (dlg.Icon == null)
            dlg.Icon = Application.Current.MainWindow.Icon;
        }
        if (isDefaultPosition)
          dlg.WindowStartupLocation = WindowStartupLocation.CenterOwner;
      }

      dlg.ShowInTaskbar = false;  // model dialog should not show icon in the task bar.
      using (new CursorSetter(null))
      {
        WindowsManager.Current.AddDialog(dlg);
        bool? result = dlg.ShowDialog();
        WindowsManager.Current.RemoveDialog(dlg);

        if (isEnabledOld != null)
          OS.Win32.EnableWindow(OS.Win32.GetActiveWindow(), isEnabledOld ?? true);

        dlg.Loaded -= new RoutedEventHandler(OnBrowserApplicationDialog_Loaded);
        dlg.Topmost = isTopmostOld;

        return result;
      }
    }
    private static void OnBrowserApplicationDialog_Loaded(object sender, RoutedEventArgs e)
    {
      Window dlg = sender as Window;
      if (dlg == null)
        return;
      if (!IsBrowserApplication)
        return;

      if (Application.Current != null && Application.Current.MainWindow != null)
      {
        Point orginalPoint = Application.Current.MainWindow.PointToScreen(new Point(0, 0));
        dlg.WindowStartupLocation = WindowStartupLocation.Manual;
        dlg.Left = orginalPoint.X + (Application.Current.MainWindow.Width - dlg.Width) / 2;
        dlg.Top = orginalPoint.Y + (Application.Current.MainWindow.Height - dlg.Height) / 2;
      }
    }

    public static bool IsWindows2000
    {
      get { return (Environment.OSVersion.Platform == PlatformID.Win32NT) && (Environment.OSVersion.Version.Major == 5) && (Environment.OSVersion.Version.Minor == 0); }
    }
    public static bool IsWindowsXP
    {
      get { return (Environment.OSVersion.Platform == PlatformID.Win32NT) && (Environment.OSVersion.Version.Major == 5) && (Environment.OSVersion.Version.Minor == 1); }
    }
    public static bool IsWindows2003
    {
      get { return (Environment.OSVersion.Platform == PlatformID.Win32NT) && (Environment.OSVersion.Version.Major == 5) && (Environment.OSVersion.Version.Minor == 2); }
    }
    public static bool IsWindowsVista
    {
      get { return (Environment.OSVersion.Platform == PlatformID.Win32NT) && (Environment.OSVersion.Version.Major == 6) && (Environment.OSVersion.Version.Minor == 0); }
    }
    public static bool IsWindows7
    {
      get { return (Environment.OSVersion.Platform == PlatformID.Win32NT) && (Environment.OSVersion.Version.Major == 6) && (Environment.OSVersion.Version.Minor == 1); }
    }
    public static bool IsWindows8
    {
      get { return (Environment.OSVersion.Platform == PlatformID.Win32NT) && (Environment.OSVersion.Version.Major == 6) && (Environment.OSVersion.Version.Minor == 2); }
    }

    public static string AppDataDirectory
    {
      get
      {
        string ret = string.Empty;
        if (IsWindows2000 || IsWindowsXP || IsWindows2003)
          ret = string.Format("{0}\\DataPage+", Environment.GetEnvironmentVariable("ALLUSERSPROFILE") + "\\Documents\\Hexagon");
        else
          ret = string.Format("{0}\\DataPage+", Environment.GetEnvironmentVariable("PUBLIC") + "\\Documents\\Hexagon");
        ForceCreateDirectory(ret);
        return ret;
      }
    }
    public static string ForceCreateDirectory(string directoryPath)
    {
      if (String.IsNullOrEmpty(directoryPath))
        return String.Empty;

      string pathRoot = System.IO.Path.GetPathRoot(directoryPath);
      if (string.IsNullOrEmpty(pathRoot))
        directoryPath = string.Format("{0}\\{1}", AppDataDirectory, directoryPath);

      string parentPath = System.IO.Path.GetDirectoryName(directoryPath);
      if (!System.IO.Directory.Exists(parentPath))
        ForceCreateDirectory(parentPath);

      if (!System.IO.Directory.Exists(directoryPath))
        System.IO.Directory.CreateDirectory(directoryPath);

      return directoryPath;
    }
    public static string ForceCreateDirectoryEx(string filePath)
    {
      return ForceCreateDirectory(System.IO.Path.GetDirectoryName(filePath));
    }
    public static System.Windows.Forms.Screen GetScreen(Window window)
    {
      return System.Windows.Forms.Screen.FromHandle(new WindowInteropHelper(window).Handle);
    }
    public static Rect GetWorkArea(DependencyObject obj)
    {
      var window = obj as Window;
      if (window == null)
        window = Window.GetWindow(obj);
      if (window == null)
        return SystemParameters.WorkArea;
      return GetWorkArea(window);
    }
    public static Rect GetWorkArea(Window window)
    {
      try
      {
        var rectangle = GetScreen(window).WorkingArea;
        return new Rect(new Size(PixelsToDIU(rectangle.Width), PixelsToDIU(rectangle.Height)));
      }
      catch (Exception)
      {
        return SystemParameters.WorkArea;
      }
    }

    private static string _PreviousInputLanguageName = string.Empty;
    public static void SetInputMethod(DisplayModes keyboardDisplayMode, bool isFull)
    {
      foreach (CultureInfo lang in InputLanguageManager.Current.AvailableInputLanguages)
      {
        if (keyboardDisplayMode == DisplayModes.Japanese)
        {
          if (lang.TwoLetterISOLanguageName == "ja")
          {
            if (InputLanguageManager.Current.CurrentInputLanguage.TwoLetterISOLanguageName != "ja")
              _PreviousInputLanguageName = InputLanguageManager.Current.CurrentInputLanguage.TwoLetterISOLanguageName;
            InputLanguageManager.Current.CurrentInputLanguage = lang;

            InputMethod.Current.ImeSentenceMode = ImeSentenceModeValues.PhrasePrediction;
            InputMethod.Current.ImeConversionMode = ImeConversionModeValues.FullShape | ImeConversionModeValues.Native | ImeConversionModeValues.Roman;
            InputMethod.Current.ImeState = isFull ? InputMethodState.On : InputMethodState.Off;
          }
        }
        else
        {
          if (lang.TwoLetterISOLanguageName == _PreviousInputLanguageName)
          {
            InputLanguageManager.Current.CurrentInputLanguage = lang;
            InputMethod.Current.ImeState = InputMethodState.Off;
            break;
          }
        }
      }
    }
  }

  // pixel
  // System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width
  // System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height

  // WPF unit
  // SystemParameters.PrimaryScreenWidth
  // SystemParameters.PrimaryScreenHeight
  // SystemParameters.WorkArea.Size.Width
  // SystemParameters.WorkArea.Size.Height
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
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

namespace Hexagon.Software.NCGage.CustomControls
{
  [TemplatePart(Name = Part_BorderPath, Type = typeof(Path))]
  [TemplatePart(Name = PART_TrianglePath, Type = typeof(Path))]
  [TemplatePart(Name = PART_Image, Type = typeof(Image))]
  public class FanucButton : ContentControl, ICommandSource
  {
    private void RaiseCommand()
    {
      if (Command != null)
      {
        RoutedCommand rc = Command as RoutedCommand;
        if (rc != null)
          rc.Execute(CommandParameter, CommandTarget);
        else
          Command.Execute(CommandParameter);
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

    private Boolean DrawContent(int width, int height, int alpha)
    {
      try
      {
        var path = this.GetTemplateChild(Part_BorderPath) as Path;
        var pathTriangle = this.GetTemplateChild(PART_TrianglePath) as Path;
        if (path == null || pathTriangle == null)
          return false;

        var figureString = String.Format("M {0},0 H {1} V {3} L {4},{2} H 0 V {0} Z", alpha, width, height, height - alpha, width - alpha);

        // Zehong: the following line doesn't support Transform
        //path.Data = PathGeometry.Parse(figureString);
        path.Data = new PathGeometry() { Figures = PathFigureCollection.Parse(figureString) }; // draw border
        path.Data.Transform = this._shapeTransform;
        // draw lower-right triangle
        var triangleFigures = String.Format("M {0},{5} v {1} l {4},{2} h {3} Z", width, LicenseThickness, alpha, -LicenseThickness, -alpha, height - alpha - LicenseThickness);
        pathTriangle.Data = new PathGeometry() { Figures = PathFigureCollection.Parse(triangleFigures) };
        pathTriangle.Data.Transform = this._shapeTransform;
        return true;
      }
      catch (Exception)
      {
        return false;
      }
    }
    private void UpdateImageSize(Double alpha)
    {
      var image = this.GetTemplateChild(PART_Image) as Image;
      if (image == null)
        return;

      var newMargin = alpha * 2;
      var diffWidth = image.Margin.Left + image.Margin.Right - newMargin;
      var diffHeight = image.Margin.Top + image.Margin.Bottom - newMargin;
      if (diffWidth < 0.000001 && diffHeight < 0.000001)
        return;

      // set image Margin, Width & Height
      image.Margin = new Thickness(alpha);
      image.Width = image.ActualWidth + diffWidth;
      image.Height = image.ActualHeight + diffHeight;
    }

    protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
    {
      try
      {
        if (sizeInfo != null)
          base.OnRenderSizeChanged(sizeInfo);

        int width = (int)Math.Max(this.ActualWidth - this.Margin.Top - this.Margin.Bottom, 0);
        int height = (int)Math.Max(this.ActualHeight - this.Margin.Left - this.Margin.Right, 0);
        int alpha = Math.Max(height / 10, 5);
        this.UpdateImageSize(alpha);
        if (_drawingSize == null)
        {
          if (DrawContent(width, height, alpha))
            _drawingSize = new Size(width, height);
        }
        else
        {
          Matrix shapeMatrix = Matrix.Identity;
          shapeMatrix.Scale(width / _drawingSize.Value.Width, height / _drawingSize.Value.Height);
          this._shapeTransform.Matrix = shapeMatrix;
        }
      }
      catch (Exception) { }
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

        this.RaiseEvent(new RoutedEventArgs(ClickEvent, this));
        this.RaiseCommand();
      }
      catch (Exception) { }
      finally
      {
        this.IsPressed = false;
      }
    }

    #region Events
    private static readonly RoutedEvent ClickEvent =
      EventManager.RegisterRoutedEvent("Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(FanucButton));
    public event RoutedEventHandler Click
    {
      add { AddHandler(ClickEvent, value); }
      remove { RemoveHandler(ClickEvent, value); }
    }

    #endregion

    #region Dependency Properties
    //[TypeConverterAttribute(typeof(CommandConverter))]
    [TypeConverter(typeof(CommandConverter))]
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
    public NCFunctionNames FunctionName
    {
      get { return (NCFunctionNames)this.GetValue(FunctionNameProperty); }
      set { this.SetValue(FunctionNameProperty, value); }
    }
    public ImageSource EnabledImage
    {
      get { return (ImageSource)this.GetValue(EnabledImageProperty); }
      set { this.SetValue(EnabledImageProperty, value); }
    }
    public ImageSource DisabledImage
    {
      get { return (ImageSource)this.GetValue(DisabledImageProperty); }
      set { this.SetValue(DisabledImageProperty, value); }
    }
    public bool IsPressed
    {
      get { return (bool)GetValue(IsPressedProperty); }
      set { SetValue(IsPressedProperty, value); }
    }
    public double LicenseThickness
    {
      get { return (double)GetValue(LicenseThicknessProperty); }
      set { SetValue(LicenseThicknessProperty, value); }
    }
    public LicenseModes LicenseMode
    {
      get { return (LicenseModes)GetValue(LicenseModeProperty); }
      set { SetValue(LicenseModeProperty, value); }
    }

    private static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(FanucButton),
      new PropertyMetadata((ICommand)null, OnCommandChanged));
    private static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register("CommandParameter", typeof(object), typeof(FanucButton), new PropertyMetadata(null));
    private static readonly DependencyProperty CommandTargetProperty = DependencyProperty.Register("CommandTarget", typeof(IInputElement), typeof(FanucButton), new PropertyMetadata(null));

    private static readonly DependencyProperty FunctionNameProperty = DependencyProperty.Register("FunctionName", typeof(NCFunctionNames), typeof(FanucButton),
      new PropertyMetadata(NCFunctionNames.None, OnFunctionNameChanged));
    private static readonly DependencyProperty EnabledImageProperty = DependencyProperty.Register("EnabledImage", typeof(ImageSource), typeof(FanucButton));
    private static readonly DependencyProperty DisabledImageProperty = DependencyProperty.Register("DisabledImage", typeof(ImageSource), typeof(FanucButton));
    private static readonly DependencyProperty IsPressedProperty = DependencyProperty.Register("IsPressed", typeof(bool), typeof(FanucButton));
    private static readonly DependencyProperty LicenseThicknessProperty = DependencyProperty.Register("LicenseThickness", typeof(double), typeof(FanucButton),
      new PropertyMetadata(6.0, OnLicenseThicknessChanged));
    public static readonly DependencyProperty LicenseModeProperty = DependencyProperty.Register("LicenseMode", typeof(LicenseModes), typeof(FanucButton), new PropertyMetadata(LicenseModes.License));


    private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var control = d as FanucButton;
      if (control != null)
        control.OnCommandChanged((ICommand)e.OldValue, (ICommand)e.NewValue);
    }
    private static void OnFunctionNameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      try
      {
        var button = d as FanucButton;
        if (button == null)
          return;

        var buttonTypeFullName = d.GetType().FullName;
        var funtionTypeFullName = String.Format("{0}.{1}Function", buttonTypeFullName.Substring(0, buttonTypeFullName.LastIndexOf('.')), button.FunctionName);
        var funtionType = Type.GetType(funtionTypeFullName);
        var function = Activator.CreateInstance(funtionType) as INCFunction;
        if (function != null)
          button.LicenseMode = function.GetLicenseMode();
      }
      catch (Exception) { }
    }
    private static void OnLicenseThicknessChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      try
      {
        var button = d as FanucButton;
        if (button != null)
          button.OnRenderSizeChanged(null);
      }
      catch (Exception) { }
    }

    #endregion

    #region Variables
    private EventHandler _canExecuteChangedHandler; // keeps a copy of the CanExecuteChanged handler so it doesn't get garbage collected
    private Size? _drawingSize = null;
    private MatrixTransform _shapeTransform = new MatrixTransform();
    #endregion

    #region const & static
    const String Part_BorderPath = "PART_BorderPath";
    const String PART_TrianglePath = "PART_TrianglePath";
    const String PART_Image = "PART_Image";
    static FanucButton()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(FanucButton), new FrameworkPropertyMetadata(typeof(FanucButton)));
    }
    #endregion
  }

  public interface INCFunction
  {
    LicenseModes GetLicenseMode();
  }
  public class CalibrationFunction : INCFunction
  {
    public LicenseModes GetLicenseMode()
    {
      return LicenseModes.License;
    }
  }
  public class SetOffsetFunction : INCFunction
  {
    public LicenseModes GetLicenseMode()
    {
      return LicenseModes.License;
    }
  }
  public class MeasurementFunction : INCFunction
  {
    public LicenseModes GetLicenseMode()
    {
      return LicenseModes.License;
    }
  }
  public class ReportsFunction : INCFunction
  {
    public LicenseModes GetLicenseMode()
    {
      return LicenseModes.Enabled;
    }
  }
  public class StatisticsFunction : INCFunction
  {
    public LicenseModes GetLicenseMode()
    {
      return LicenseModes.Disabled;
    }
  }

  public enum LicenseModes { Disabled = 0, Enabled = 1, License = 2 }
  public enum NCFunctionNames
  {
    None = 0,
    Calibration = 1,
    SetOffset = 2,
    Measurement = 3,
    Reports = 4,
    Statistics = 5
  }

  public sealed class ThicknessToDoubleConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (!(value is Thickness))
        return 0;
      return ((Thickness)value).Left;
    }
    object IValueConverter.ConvertBack(object value, Type type, object parameter, CultureInfo culture)
    {
      var doubleValue = 0.0;
      Double.TryParse((value ?? "0").ToString(), out doubleValue);
      return new Thickness(doubleValue);
    }
  }
  public sealed class EnumToVisibilityConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value == null || parameter == null)
        return Visibility.Visible;
      if (object.ReferenceEquals(value, parameter) || object.Equals(value, parameter))
        return Visibility.Visible;

      if (!value.GetType().IsEnum)
        throw new NotSupportedException();

      if (!Enum.IsDefined(value.GetType(), parameter.ToString()))
        return Visibility.Collapsed;

      return (value.ToString() == Enum.Parse(value.GetType(), parameter.ToString()).ToString()) ? Visibility.Visible : Visibility.Collapsed;
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotSupportedException();
    }
  }
  public sealed class DisabledToDoubleCollectionConverter : IValueConverter
  {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
      if (value == null)
        return null;

      var mode = LicenseModes.License;
      if (Enum.TryParse(value.ToString(), out mode) && mode == LicenseModes.Disabled)
      {
        try
        {
          return DoubleCollection.Parse(parameter.ToString());
        }
        catch (Exception)
        {
          return DoubleCollection.Parse("2");
        }
      }
      else
        return null;
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
      throw new NotSupportedException();
    }
  }
}

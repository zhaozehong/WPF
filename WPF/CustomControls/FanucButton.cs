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

namespace WPF.CustomControls
{
  [TemplatePart(Name = PART_RootGrid, Type = typeof(UIElement))]
  [TemplatePart(Name = Part_BorderPath, Type = typeof(Path))]
  [TemplatePart(Name = PART_Image, Type = typeof(Image))]
  public class FanucButton : ContentControl
  {
    const String PART_RootGrid = "PART_RootGrid";
    const String Part_BorderPath = "PART_BorderPath";
    const String PART_Image = "PART_Image";
    static FanucButton()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(FanucButton), new FrameworkPropertyMetadata(typeof(FanucButton)));
    }

    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();

      var uiElement = this.GetTemplateChild(PART_RootGrid) as UIElement;
      if (uiElement != null)
      {
        uiElement.MouseLeftButtonDown += UiElement_MouseLeftButtonDown;
        uiElement.MouseLeftButtonUp += UiElement_MouseLeftButtonUp;
      }
    }

    private void UiElement_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      this.IsPressed = true;
    }
    private void UiElement_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
    {
      this.IsPressed = false;
      var args = new RoutedEventArgs(ClickEvent);
      this.RaiseEvent(args);
    }

    protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
    {
      if (sizeInfo != null)
        base.OnRenderSizeChanged(sizeInfo);

      var path = this.GetTemplateChild(Part_BorderPath) as Path;
      var pathTriangle = this.GetTemplateChild("PART_TrianglePath") as Path;
      if (path == null || pathTriangle == null)
        return;

      int height = (int)Math.Max(this.ActualHeight - this.Margin.Left - this.Margin.Right, 0);
      int width = (int)Math.Max(this.ActualWidth - this.Margin.Top - this.Margin.Bottom, 0);
      int alpha = Math.Max(height / 10, 5);
      var figureString = String.Format("M {0},0 H {1} V {3} L {4},{2} H 0 V {0} Z", alpha, width, height, height - alpha, width - alpha);
      path.Data = PathGeometry.Parse(figureString); // draw border
      if (this.LicenseMode == LicenseModes.License)
      {
        // draw lower-right triangle
        var thickness = 5;
        var triangleFigures = String.Format("M {0},{5} v {1} l {4},{2} h {3} Z", width, thickness, alpha, -thickness, -alpha, height - alpha - thickness);
        pathTriangle.Data = PathGeometry.Parse(triangleFigures);
      }
      else
      {
        // clear lower-right triangle
        if (pathTriangle.Data != null)
          pathTriangle.Data = PathGeometry.Parse("M 0,0");

        // draw dotted style line
        if (this.LicenseMode == LicenseModes.Disabled)
          path.StrokeDashArray = DoubleCollection.Parse("2");
      }

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

    #region Events
    private static readonly RoutedEvent ClickEvent =
      EventManager.RegisterRoutedEvent("Click", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(FanucButton));
    public event RoutedEventHandler Click
    {
      add { AddHandler(ClickEvent, value); }
      remove { RemoveHandler(ClickEvent, value); }
    }

    #endregion

    #region .Net/CLR Properties
    public LicenseModes LicenseMode
    {
      get { return _licenseMode; }
      private set
      {
        if (_licenseMode != value)
        {
          _licenseMode = value;
          this.OnRenderSizeChanged(null);
        }
      }
    }
    #endregion

    #region Dependency Properties
    public NCFunctionNames FunctionName
    {
      get { return (NCFunctionNames)this.GetValue(FunctionNameProperty); }
      set { this.SetValue(FunctionNameProperty, value); }
    }
    public ImageSource ImageSource
    {
      get { return (ImageSource)this.GetValue(ImageSourceProperty); }
      set { this.SetValue(ImageSourceProperty, value); }
    }
    public ImageSource ImageSourceOff
    {
      get { return (ImageSource)this.GetValue(ImageSourceOffProperty); }
      set { this.SetValue(ImageSourceOffProperty, value); }
    }
    public bool IsPressed
    {
      get { return (bool)GetValue(IsPressedProperty); }
      set { SetValue(IsPressedProperty, value); }
    }

    public static readonly DependencyProperty FunctionNameProperty =
      DependencyProperty.Register("FunctionName", typeof(NCFunctionNames), typeof(FanucButton), new PropertyMetadata(NCFunctionNames.None, OnFunctionNameChanged));
    public static readonly DependencyProperty ImageSourceProperty =
      DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(FanucButton));
    public static readonly DependencyProperty ImageSourceOffProperty =
      DependencyProperty.Register("ImageSourceOff", typeof(ImageSource), typeof(FanucButton));
    private static readonly DependencyProperty IsPressedProperty =
        DependencyProperty.Register("IsPressed", typeof(bool), typeof(FanucButton), new PropertyMetadata(false));

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

    #endregion

    #region Variables
    private LicenseModes _licenseMode = LicenseModes.License;
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
}

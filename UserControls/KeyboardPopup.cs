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
  public class KeyboardPopup : PopupEx
  {
    static KeyboardPopup()
    {
      //DefaultStyleKeyProperty.OverrideMetadata(typeof(KeyboardPopup), new FrameworkPropertyMetadata(typeof(KeyboardPopup)));
    }
    private void CalculatorKeyboard_Closed(object sender, EventArgs e)
    {
      var keyboard = sender as CalculatorKeyboard;
      if (keyboard != null)
      {
        //var popup = keyboard.Parent as PopupEx;
        //  if(popup!= null)
        //  popup
      }
    }


    #region Dependency Properties
    public double ButtonSize
    {
      get { return (Double)GetValue(ButtonSizeProperty); }
      set { SetValue(ButtonSizeProperty, value); }
    }
    public static readonly DependencyProperty ButtonSizeProperty = DependencyProperty.Register("ButtonSize", typeof(double), typeof(CalculatorKeyboard), new PropertyMetadata(50.0));

    public TextBox TargetElement
    {
      get { return (TextBox)GetValue(TargetElementProperty); }
      set { SetValue(TargetElementProperty, value); }
    }
    public static readonly DependencyProperty TargetElementProperty = DependencyProperty.Register("TargetElement", typeof(TextBox), typeof(CalculatorKeyboard), new PropertyMetadata(null));

    #endregion
  }
}

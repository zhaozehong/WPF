using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace Helper
{
  public class ShowNotificationMessageBehavior : Behavior<ContentControl>
  {
    public String Message
    {
      get { return (String)this.GetValue(MessageProperty); }
      set { this.SetValue(MessageProperty, value); }
    }
    public static readonly DependencyProperty MessageProperty =
      DependencyProperty.Register("Message", typeof(string),
        typeof(ShowNotificationMessageBehavior), new PropertyMetadata(null, OnMessageChanged));

    private static void OnMessageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var behavior = d as ShowNotificationMessageBehavior;
      behavior.AssociatedObject.Content = e.NewValue;
      behavior.AssociatedObject.Visibility = Visibility.Visible;
    }

    protected override void OnAttached()
    {
      AssociatedObject.MouseLeftButtonDown += (s, e) => AssociatedObject.Visibility = Visibility.Collapsed;
    }
  }
}

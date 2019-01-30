using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace WPF.ViewModel
{
  public class NotifyPropertyChanged : INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(String propertyName)
    {
      var propertyChanged = PropertyChanged;
      if (propertyChanged != null)
      {
        propertyChanged(this, new PropertyChangedEventArgs(propertyName));
      }
    }

    protected void OnPropertyChanged<T>(Expression<Func<T>> projection)
    {
      var memberExpression = (MemberExpression)projection.Body;
      this.OnPropertyChanged(memberExpression.Member.Name);
    }
    public string GetPropertyName<T>(Expression<Func<T>> projection)
    {
      var memberExpression = (MemberExpression)projection.Body;
      return (memberExpression.Member.Name);
    }
  }
}

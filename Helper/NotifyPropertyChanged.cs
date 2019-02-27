using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Helper
{
  public abstract class NotifyPropertyChanged : INotifyPropertyChanged
  {
    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void RaisePropertyChanged(String propertyName)
    {
      this.VerifyPropertyName(propertyName);

      PropertyChangedEventHandler handler = null;
      lock (this)
      {
        handler = PropertyChanged;
      }
      if (handler != null)
        handler(this, new PropertyChangedEventArgs(propertyName));
    }
    protected void RaisePropertyChanged<TProperty>(Expression<Func<TProperty>> propertyExpresssion)
    {
      var memberExpression = (MemberExpression)propertyExpresssion.Body;
      RaisePropertyChanged(memberExpression.Member.Name);
    }
    public string GetPropertyName<TProperty>(Expression<Func<TProperty>> projection)
    {
      var memberExpression = (MemberExpression)projection.Body;
      return (memberExpression.Member.Name);
    }
    [Conditional("DEBUG")]
    [DebuggerStepThrough]
    public void VerifyPropertyName(String propertyName)
    {
      if (TypeDescriptor.GetProperties(this)[propertyName] == null)
      {
        Debug.Fail("Invalid property name: " + propertyName);
      }
    }
  }
}

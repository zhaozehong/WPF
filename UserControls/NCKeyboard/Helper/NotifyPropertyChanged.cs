using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WPF.Helper
{
  public abstract class NotifyPropertyChanged : INotifyPropertyChanged
  {
    protected virtual void SetProperty<T>(ref T member, T val, [CallerMemberName] string propertyName = null)
    {
      if (object.Equals(member, val))
        return;

      member = val;
      RaisePropertyChanged(propertyName);
    }
    protected void RaisePropertyChanged<TProperty>(Expression<Func<TProperty>> propertyExpresssion)
    {
      try
      {
        var memberExpression = (MemberExpression)propertyExpresssion.Body;
        RaisePropertyChanged(memberExpression.Member.Name);
      }
      catch (Exception) { }
    }
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

    public string GetPropertyName<TProperty>(Expression<Func<TProperty>> propertyExpresssion)
    {
      try
      {
        var memberExpression = (MemberExpression)propertyExpresssion.Body;
        return memberExpression.Member.Name;
      }
      catch (Exception)
      {
        return null;
      }
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


    //public event PropertyChangedEventHandler PropertyChanged = delegate { };
    public event PropertyChangedEventHandler PropertyChanged;
  }
}

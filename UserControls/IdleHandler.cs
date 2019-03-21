using System;
using System.Windows.Threading;

namespace Hexagon.Software.NCGage.HelperLib
{
  public abstract class IdleHandler : DispatcherObject
  {
    protected IdleHandler()
    {
      WindowsManager.Current.AddIdleHandler(this);
    }
    public void Start()
    {
      WindowsManager.Current.StartIdle();
    }
    public DateTime DateTimeLastExccute = DateTime.MinValue;
    public abstract void DealWithIdle(double nIdleSeconds);
    public abstract bool IsEnabled { get; }
    public abstract double MaxIdleInterval { get; }
  }
}

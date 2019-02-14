using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Interop;

namespace XbapDemo
{
  [ComVisible(true)]
  public class CallbackContainer
  {
    private SynchronizationContext sync = SynchronizationContext.Current;
    [DispId(0)]
    public string GetMessage(/*dynamic arg*/)
    {
      // go back to UI thread
      sync.Post(
        delegate
        {
          var hostScript = BrowserInteropHelper.HostScript;
          // ...
        },
        null);
      return "I'll call you back...";

      //return "Message: " + arg.Foo + ", " + arg.Bar;
    }
  }
}

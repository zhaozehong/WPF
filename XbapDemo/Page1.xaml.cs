using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace XbapDemo
{
  public partial class Page1 : Page
  {
    public Page1()
    {
      InitializeComponent();
    }

    private void Button_Click(object sender, RoutedEventArgs e)
    {
      if (!BrowserInteropHelper.IsBrowserHosted)
      {
        MessageBox.Show("不满足与JS调用条件");
        return;
      }

      dynamic hostScript = BrowserInteropHelper.HostScript;
      if (hostScript == null)
      {
        messageTextBlock.Text = "HostScript is null";
      }
      else
      {
        messageTextBlock.Text = "HostScript found!";

        //hostScript.TestMethod(new MyData { Name = "Ian", Age = 37.5 });

        //var jsObj = hostScript.ReturnJsObject();
        var jsObj = hostScript.myObj;

        String foo = jsObj.Foo;
        int bar = jsObj.Bar;
        Debug.WriteLine(foo);
        Debug.WriteLine(bar);

        jsObj.Foo = "Modified by C#";
        jsObj.Bar = 55321;

        //jsObj.Operation(new MyData { Name = "Ian", Age = 37.5 });
        //hostScript.ShowObj();

        //hostScript.CallMethod(new CallbackContainer());

        dynamic document = hostScript.document;
        var Button1 = document.getElementById("Button1");
        Button1.onclick = new CallbackContainer();
      }
    }
  }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Zza.Services;

namespace Zza.SelfHost
{
  class Program
  {
    static void Main(string[] args)
    {
      try
      {
        ServiceHost host = new ServiceHost(typeof(ZzaService));
        host.Open();

        Console.ReadKey();
        host.Close();
      }
      catch (Exception ex)
      {
        Debug.WriteLine(ex.Message);
      }
      
    }
  }
}

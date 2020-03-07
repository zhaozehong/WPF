using System;
using System.Diagnostics;
using System.Threading;
using Helper;
using System.Linq;

namespace ConsoleApp2
{
  class Program
  {
    static void Main(string[] args)
    {
      var random = new Random();
      var count = 200000;
      var array = new int[count];
      for (int i = 0; i < count; i++)
      {
        array[i] = random.Next();
      }
      //var array = new int[] { 72, 6, 57, 88, 60, 42, 83, 73, 48, 85, 100, 4, 54, 4245, 2435, 523, 45, 86, 864, 23453, 643, 674 };

      //Console.WriteLine("Before Sort:");
      //foreach (var item in array)
      //  Console.Write(item + "  ");

      var watch = new Stopwatch();
      watch.Start();

      //AppHelper.SortArrayByQuick(array);
      //AppHelper.SortArrayByMerge(array);
      //AppHelper.SortArrayByBubble(array);
      array = array.OrderBy(p => p).ToArray();
      Console.WriteLine("[Total Time]:" + watch.Elapsed.TotalSeconds.ToString());

      //Console.WriteLine("\r\nAfter Sort:");
      //foreach (var item in array)
      //  Console.Write(item + "  ");
      //for (int i = 0; i < array.Length; i++)
      //{
      //  if (i % 23 == 22)
      //    Console.WriteLine(array[i] + "  ");
      //  else
      //    Console.Write(array[i] + "  ");
      //}
      Console.ReadKey();
    }
  }
}

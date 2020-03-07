using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Helper
{
  public static class AppHelper
  {
    #region Sort by Quick
    public static void SortArrayByQuick(int[] array, int startIndex = -1, int endIndex = -1)
    {
      if (startIndex == -1)
        startIndex = 0;
      if (endIndex == -1)
        endIndex = array.Length - 1;

      QuickSort(array, startIndex, endIndex);
    }
    private static void QuickSort(int[] array, int iLeft, int iRight)
    {
      if (iLeft >= iRight)
        return;

      iLeft = Math.Max(iLeft, 0);
      iRight = Math.Min(iRight, array.Length - 1);

      if (iLeft < iRight)
      {
        int l = iLeft, r = iRight, x = array[iLeft];
        while (l < r)
        {
          while (l < r && array[r] >= x) // find the first value smaller than X from R to L
            r--;
          if (l < r)
            array[l++] = array[r];

          while (l < r && array[l] < x) // find the first value larger than X from L to R
            l++;
          if (l < r)
            array[r--] = array[l];
        }
        array[l] = x;
        QuickSort(array, iLeft, l - 1);
        QuickSort(array, l + 1, iRight);
      }
    }
    #endregion

    #region Sort by MergeMethod
    public static void SortArrayByMerge(int[] array, int startIndex = -1, int endIndex = -1)
    {
      if (startIndex == -1)
        startIndex = 0;
      if (endIndex == -1)
        endIndex = array.Length - 1;
      MergeSort(array, startIndex, endIndex);
    }

    private static void MergeSort(int[] array, int iFirst, int iLast, int[] temp = null)
    {
      if (iFirst >= iLast)
        return;

      iFirst = Math.Max(iFirst, 0);
      iLast = Math.Min(iLast, array.Length - 1);
      if (temp == null)
        temp = new int[array.Length];

      var iMiddle = (iFirst + iLast) / 2;
      MergeSort(array, iFirst, iMiddle, temp); // sort left array
      MergeSort(array, iMiddle + 1, iLast, temp); // sort right array

      MergeArray(array, iFirst, iMiddle, iLast, temp); // merge the sorted arrays
    }
    private static void MergeArray(int[] array, int first, int mid, int last, int[] temp)
    {
      int i = first, j = mid + 1;
      int m = mid, n = last;
      int k = 0;

      while (i <= m && j <= n)
      {
        if (array[i] <= array[j])
          temp[k++] = array[i++];
        else
          temp[k++] = array[j++];
      }

      while (i <= m)
        temp[k++] = array[i++];

      while (j <= n)
        temp[k++] = array[j++];

      for (i = 0; i < k; i++)
        array[first + i] = temp[i];
    }
    #endregion

    

    #region Sort by Bubble
    public static void SortArrayByBubble(int[] array)
    {
      int count = array.Length;
      for (int i = 1; i < count; i++)
      {
        for (int j = 0; j < count - i; j++)
        {
          if (array[j] > array[j + 1])
          {
            //交换
            int buffer = array[j];
            array[j] = array[j + 1];
            array[j + 1] = buffer;
          }
        }
      }
    }
    #endregion
  }
}

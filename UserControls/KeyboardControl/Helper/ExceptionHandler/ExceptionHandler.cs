using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hexagon.Software.NCGage.HelperLib
{
  public class ExceptionHandler
  {
    public static void ThrowException(Exception e)
    {
      if (e is System.Threading.ThreadAbortException)
        return;
      CurrentExceptionManager.ThrowException(e);
    }

    public static void ThrowException(String error, bool needDumpStackTrace = true)
    {
      CurrentExceptionManager.ThrowException(error, needDumpStackTrace);
    }

    public static void LogFile(String info)
    {
      CurrentExceptionManager.LogFile(info);
    }

    public static void ShowFinishOperationMessageBox(string opName)
    {
      CurrentExceptionManager.ShowFinishOperationMessageBox(opName);
    }

    public static void ShowErrorMessageBox()
    {
      CurrentExceptionManager.ShowErrorMessageBox();
    }

    public static void SetExtraInfomation(string extraInfo)
    {
      CurrentExceptionManager.SetExtraInfomation(extraInfo);
    }

    public static void ShowErrorMessageBox(string caption)
    {
      CurrentExceptionManager.ShowErrorMessageBox(caption);
    }

    public static void ClearAllExceptions()
    {
      CurrentExceptionManager.ClearAllExceptions();
    }

    public static void ClearException(String exception)
    {
      CurrentExceptionManager.ClearException(exception);
    }

    public static bool IsIgnoreWarningMessage
    {
      get { return CurrentExceptionManager.IsIgnoreWarningMessage; }
      set { CurrentExceptionManager.IsIgnoreWarningMessage = value; }
    }

    public static string AllErrorMessage
    {
      get { return CurrentExceptionManager.AllErrorMessage; }
    }

    public static bool HasError
    {
      get { return CurrentExceptionManager.HasError; }
    }

    public static String LastErrorMessage
    {
      get { return CurrentExceptionManager.LastErrorMessage; }
    }

    private static IExceptionManager _currentExceptionManager = null;
    public static IExceptionManager CurrentExceptionManager
    {
      get
      {
        if (_currentExceptionManager == null)
          _currentExceptionManager = DispatcherObjectExternsion.SafeCreateInstance<DefaultExceptionManager>();
        return _currentExceptionManager;
      }
      set { _currentExceptionManager = value; }
    }
  }
}

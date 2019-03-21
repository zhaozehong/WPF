using System;

namespace Hexagon.Software.NCGage.HelperLib
{
  public interface IExceptionManager
  {
    void ThrowException(Exception e);
    void ThrowException(string error, bool needDumpStackTrace);

    void LogFile(string info);

    void ShowFinishOperationMessageBox(string opName);

    void ShowErrorMessageBox();
    void ShowErrorMessageBox(string caption);

    void SetExtraInfomation(string extraInfo);

    void ClearAllExceptions();
    void ClearException(string exception);

    bool IsIgnoreWarningMessage { get; set; }

    string AllErrorMessage { get; }

    bool HasError { get; }

    string LastErrorMessage { get; }
  }
}

using Hexagon.Software.NCGage.UserControls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Threading;

namespace Hexagon.Software.NCGage.HelperLib
{
  public class DefaultExceptionManager : DispatcherObject, IExceptionManager
  {
    public DefaultExceptionManager()
    {
      IsIgnoreWarningMessage = false;
    }

    #region IExceptionManager Members

    public void ThrowException(Exception e)
    {
      ThrowException(e.Message, false);
      if (e.InnerException != null)
        ThrowException(e.InnerException.Message);
      DumpIntoFile(e.StackTrace, false);
    }

    public void ThrowException(String error, bool needDumpStackTrace = true)
    {
      if (_latestErrorMessages.IndexOf(error) == -1)
        _latestErrorMessages.Add(error);

      DumpIntoFile(error, needDumpStackTrace);
    }

    public void LogFile(String info)
    {
      DumpIntoFile(info, false);
    }

    public void ShowFinishOperationMessageBox(string opName)
    {
      if (this.CheckAccess() == false)
      {
        this.Dispatcher.BeginInvoke(new Action(() => ShowFinishOperationMessageBox(opName)));
        return;
      }

      if (ExceptionHandler.HasError)
      {
        ExceptionHandler.ShowErrorMessageBox(opName);
      }
      else
      {
        string prompt = "Success";
        if (_extraInfomation != string.Empty)
          prompt += "\n\n" + _extraInfomation;
        _extraInfomation = string.Empty;

        var box = new MessageBoxEx
        {
          Title = opName,
          Prompt = prompt,
        };

        Helpers.ShowDialog(box);
      }
    }

    public void ShowErrorMessageBox()
    {
      if (IsIgnoreWarningMessage == true)
      {
        return;
      }

      ShowErrorMessageBox("Failed");
    }
    public void ShowErrorMessageBox(string caption)
    {
      if (this.CheckAccess() == false)
      {
        this.Dispatcher.BeginInvoke(new Action(() => ShowErrorMessageBox(caption)));
        return;
      }

      if (HasError)
      {
        string prompt = GetShortErrorMessage();
        if (_extraInfomation != string.Empty)
          prompt += "\n\n" + _extraInfomation;
        bool hasDetails = _latestErrorMessages.Count() > 12;
        string details = AllErrorMessage;

        _latestErrorMessages.Clear();
        _extraInfomation = string.Empty;

        var box = new MessageBoxEx
        {
          Title = caption,
          Prompt = prompt,
          HasDetails = hasDetails
        };
        if (box.HasDetails == true)
          box.Details = details;

        Helpers.ShowDialog(box);
      }
      else
      {
        _latestErrorMessages.Clear();
        _extraInfomation = string.Empty;
      }
    }

    public void SetExtraInfomation(string extraInfo)
    {
      _extraInfomation = extraInfo;
    }

    public void ClearAllExceptions()
    {
      _latestErrorMessages.Clear();
    }
    public void ClearException(String exception)
    {
      var query = from p in _latestErrorMessages
                  where p != exception
                  select p;
      _latestErrorMessages = query.ToList();
    }

    public bool IsIgnoreWarningMessage { get; set; }
    public string AllErrorMessage
    {
      get
      {
        string retVal = string.Empty;

        foreach (string tmp in _latestErrorMessages)
          retVal += tmp + "\n";

        retVal = retVal.TrimEnd('\n');
        return retVal;
      }
    }
    public bool HasError
    {
      get { return _latestErrorMessages.Count > 0; }
    }
    public String LastErrorMessage { get { return _latestErrorMessages.LastOrDefault(); } }

    #endregion

    private string GetAppVersionInfo()
    {
      return "NCGage";
      //if (string.IsNullOrWhiteSpace(_appVersionInfo))
      //{
      //  _appVersionInfo = string.Format("{0}{1}#{2}({3})", Helpers.ApplicationType, Helpers.GetAppVersion(),
      //                                  VersionInfo.BuildNumber.ToString(), DPApplication.Current.CurrentLanguage);
      //}
      //return _appVersionInfo;
    }
    private string GetShortErrorMessage()
    {
      string retVal = string.Empty;

      for (int i = 0; i < 12 && i < _latestErrorMessages.Count; i++)
        retVal += _latestErrorMessages[i] + "\n";

      retVal = retVal.TrimEnd('\n');
      return retVal;
    }
    private void DumpIntoFile(String info, bool needDumpStackTrace)
    {
      try
      {
        string logPath = Helpers.ForceCreateDirectory(string.Format("{0}\\log", Helpers.AppDataDirectory));
        string logFileName = string.Format("{0}\\{1}-{2}-{3}.log", logPath, DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day);
        StreamWriter sw = File.Exists(logFileName) ? File.AppendText(logFileName) : File.CreateText(logFileName);
        sw.BaseStream.Seek(0, SeekOrigin.End);
        sw.WriteLine(string.Format("****{0}****{1}", DateTime.Now, this.GetAppVersionInfo()));
        sw.WriteLine(info);
        if (needDumpStackTrace)
        {
          sw.WriteLine(string.Format("**{0}**", "StackTrace"));

          var trace = new StackTrace(true);
          var stackFrames = trace.GetFrames();
          if (stackFrames != null)
          {
            foreach (StackFrame frame in stackFrames)
            {
              string file = frame.GetFileName();
              if (string.IsNullOrEmpty(file))
                continue;
              string methodName = frame.GetMethod().Name;
              if (methodName == "DumpIntoFile" || methodName == "ThrowException")
                continue;

              int line = frame.GetFileLineNumber();
              int column = frame.GetFileColumnNumber();
              if (line == 0 && column == 0)
                continue;
              //int offset = frame.GetNativeOffset();

              sw.WriteLine(string.Format("{3} : {0}({1},{2})",
                                         System.IO.Path.GetFileName(file),
                                         line,
                                         column,
                                         methodName));
            }
          }
        }
        sw.Close();

        // Delete old files if there are more than 15 log files
        var files = Directory.GetFiles(logPath).ToList();
        if (files.Count > 15)
        {
          var dictionary = new Dictionary<String, DateTime>();
          files.ForEach(p => dictionary.Add(p, System.IO.File.GetCreationTime(p)));
          var items = dictionary.OrderBy(p => p.Value).Take(files.Count - 15);
          foreach (var item in items)
          {
            File.Delete(item.Key);
          }
        }
      }
      catch (Exception) { }
    }

    private List<string> _latestErrorMessages = new List<string>();
    private string _extraInfomation = string.Empty;
    //private string _appVersionInfo = null;
  }
}

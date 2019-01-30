using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF.Model;

namespace WPF.ViewModel
{
  public class PersonItemViewModel : NotifyPropertyChanged, IEditableObject, IDataErrorInfo
  {
    private Person _model;
    private Person _oldValue;

    public PersonItemViewModel(Person model)
    {
      _model = model;
    }

    public Person Model { get { return _model; } }
    public string FirstName
    {
      get { return _model.FirstName; }
      set
      {
        if (_model.FirstName != value)
        {
          _model.FirstName = value;
          OnPropertyChanged(() => FirstName);
        }
      }
    }
    public string LastName
    {
      get { return _model.LastName; }
      set
      {
        if (_model.LastName != value)
        {
          _model.LastName = value;
          OnPropertyChanged(() => LastName);
        }
      }
    }
    public bool IsHungry
    {
      get { return _model.IsHungry; }
      set
      {
        if (_model.IsHungry != value)
        {
          _model.IsHungry = value;
          OnPropertyChanged(() => IsHungry);
        }
      }
    }
    public PersonalityType Personality
    {
      get { return _model.Personality; }
      set
      {
        if (_model.Personality != value)
        {
          _model.Personality = value;
          OnPropertyChanged(() => Personality);
        }
      }
    }
    public Uri WebSite
    {
      get { return _model.WebSite; }
      set
      {
        if (_model.WebSite != value)
        {
          _model.WebSite = value;
          OnPropertyChanged(() => WebSite);
        }
      }
    }

    public string Error
    {
      get { return this.FirstName == this.LastName ? "Names must differ" : ""; }
    }
    public string this[string columnName]
    {
      get
      {
        switch (columnName)
        {
          case "FirstName":
            if (string.IsNullOrWhiteSpace(FirstName))
              return "First name must not be blank";
            break;
        }
        return "";
      }
    }


    public void BeginEdit()
    {
      Debug.WriteLine("BeginEdit " + FirstName + " " + LastName);
      if (_oldValue == null)
        _oldValue = _model.Copy();
    }
    public void CancelEdit()
    {
      Debug.WriteLine("CancelEdit " + FirstName + " " + LastName);
      if (_oldValue != null)
      {
        _model = _oldValue;
        _oldValue = null;

        OnPropertyChanged(() => FirstName);
        OnPropertyChanged(() => LastName);
        OnPropertyChanged(() => IsHungry);
        OnPropertyChanged(() => Personality);
        OnPropertyChanged(() => WebSite);
      }
    }
    public void EndEdit()
    {
      Debug.WriteLine("EndEdit " + FirstName + " " + LastName);
      _oldValue = null;
    }
  }
}

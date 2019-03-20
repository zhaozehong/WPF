using System;
using System.ComponentModel;
using System.Diagnostics;
using Hexagon.Software.NCGage.HelperLib;

namespace JM.Software.WPF
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
          this.RaisePropertyChanged(() => FirstName);
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
          this.RaisePropertyChanged(() => LastName);
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
          this.RaisePropertyChanged(() => IsHungry);
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
          this.RaisePropertyChanged(() => Personality);
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
          this.RaisePropertyChanged(() => WebSite);
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

        this.RaisePropertyChanged(() => FirstName);
        this.RaisePropertyChanged(() => LastName);
        this.RaisePropertyChanged(() => IsHungry);
        this.RaisePropertyChanged(() => Personality);
        this.RaisePropertyChanged(() => WebSite);
      }
    }
    public void EndEdit()
    {
      Debug.WriteLine("EndEdit " + FirstName + " " + LastName);
      _oldValue = null;
    }
  }
}

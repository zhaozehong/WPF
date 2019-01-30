using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WPF.Model;

namespace WPF.ViewModel
{
  public class PersonItemViewModel : NotifyPropertyChanged
  {
    private Person _model;
    public PersonItemViewModel(Person model)
    {
      _model = model;
    }

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

  }
}

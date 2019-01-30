using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using WPF.Model;

namespace WPF.ViewModel
{
  class PersonalityTypeViewModel
  {
    private PersonalityTypeViewModel(PersonalityType type)
    {
      this.Type = type;
    }

    public PersonalityType Type { get; private set; }

    private static ObservableCollection<PersonalityTypeViewModel> _types = new ObservableCollection<PersonalityTypeViewModel>()
    {
      new PersonalityTypeViewModel(PersonalityType.GlassHalfEmpty),
      new PersonalityTypeViewModel(PersonalityType.GlassHalfFull),
      new PersonalityTypeViewModel(PersonalityType.ItsYourRoundMate)
    };

    public static ObservableCollection<PersonalityTypeViewModel> Types { get { return _types; } }
  }
}

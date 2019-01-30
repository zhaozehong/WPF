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
      this.DisplayText = PersonalityTypeResources.ResourceManager.GetString(type.ToString() + "DisplayText");
    }

    public PersonalityType Type { get; private set; }
    public string DisplayText { get; private set; }
    private static PersonalityTypeViewModel[] _types = new PersonalityTypeViewModel[]
      {
        new PersonalityTypeViewModel(PersonalityType.GlassHalfEmpty),
        new PersonalityTypeViewModel(PersonalityType.GlassHalfFull),
        new PersonalityTypeViewModel(PersonalityType.ItsYourRoundMate)
      };

    public static IList<PersonalityTypeViewModel> Types { get { return _types; } }
  }
}

using System.Linq;
using System.Collections.ObjectModel;

namespace JM.Software.WPF
{
  public class MainWindowViewModel
  {
    public MainWindowViewModel()
    {
      var peopleViewModels = from person in Person.GetPeople()
                             select new PersonItemViewModel(person);
      People = new ObservableCollection<PersonItemViewModel>(peopleViewModels);
    }

    public ObservableCollection<PersonItemViewModel> People { get; private set; }
  }
}

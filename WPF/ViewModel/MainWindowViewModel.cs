using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using WPF.Model;

namespace WPF.ViewModel
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

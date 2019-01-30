using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPF.Model
{
  public class Person
  {
    public int ID { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public bool IsHungry { get; set; }
    public Uri WebSite { get; set; }
    public PersonalityType Personality { get; set; }

    public static IEnumerable<Person> GetPeople()
    {
      return new Person[]
      {
        new Person()
        {
          ID = 1,
          FirstName = "Ian", LastName = "Griffiths",
          WebSite = new Uri("http://www.interact-sw.co.uk/iangblog/"),
          IsHungry = true,
          Personality = PersonalityType.GlassHalfFull
        },
        new Person()
        {
          ID = 2,
          FirstName = "Jane", LastName = "Doe",
          WebSite = new Uri("http://pluralsight.com/"),
          Personality = PersonalityType.ItsYourRoundMate
        },
        new Person()
        {
          ID = 3,
          FirstName = "Ian", LastName = "Davis",
          WebSite = new Uri("http://example.com/foo/"),
          Personality = PersonalityType.GlassHalfFull
        },
        new Person()
        {
          ID = 4,
          FirstName = "Ian", LastName = "Grotbags",
          WebSite = new Uri("http://example.com/bar/"),
          Personality = PersonalityType.GlassHalfFull
        },
        new Person()
        {
          ID = 42,
          FirstName = "Arthur", LastName = "Dent",
          WebSite = new Uri("http://www.h2g2.com/"),
          IsHungry = true,
          Personality = PersonalityType.GlassHalfEmpty
        }
      };
    }
  }
}

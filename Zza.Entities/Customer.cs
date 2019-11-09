using System.Runtime.Serialization;

namespace Zza.Entities
{
  [DataContract]
  public class Customer
  {
    [DataMember]
    public int Id { get; set; }
    [DataMember]
    public string FirstName { get; set; }
    [DataMember]
    public string LastName { get; set; }
    [DataMember]
    public string Phone { get; set; }
    [DataMember]
    public string Email { get; set; }
    [DataMember]
    public string Street { get; set; }
    [DataMember]
    public string City { get; set; }
    [DataMember]
    public string State { get; set; }
    [DataMember]
    public string Zip { get; set; }
  }
}
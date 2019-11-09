using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Zza.Entities
{
  [DataContract]
  public class Product
  {
    [DataMember]
    [Key]
    public int Id { get; set; }
    [DataMember]
    public string Type { get; set; }
    [DataMember]
    public string Name { get; set; }
    [DataMember]
    public string Description { get; set; }
  }
}
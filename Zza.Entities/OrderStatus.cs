using System.ComponentModel.DataAnnotations;

namespace Zza.Entities
{
  public class OrderStatus
  {
    [Key]
    public int Id { get; set; }
    public string Name { get; set; }
  }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace Zza.Entities
{
  [DataContract]
  public class OrderItem
  {
    [Key]
    [DataMember]
    public long Id { get; set; }
    [DataMember]
    [ForeignKey("Order")]
    public long OrderId { get; set; }
    [DataMember]
    [ForeignKey("Product")]
    public int ProductId { get; set; }
    [DataMember]
    public int Quantity { get; set; }
    [DataMember]
    public decimal UnitPrice { get; set; }
    [DataMember]
    [NotMapped]
    public decimal TotalPrice { get { return Quantity * UnitPrice; } }

    public virtual Order Order { get; set; }
    public virtual Product Product { get; set; }
  }
}

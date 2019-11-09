using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace Zza.Entities
{
  [DataContract]
  public class Order
  {
    public Order()
    {
      OrderItems = new HashSet<OrderItem>();
    }

    [Key]
    [DataMember]
    public long Id { get; set; }
    [DataMember]
    [ForeignKey("Customer")]
    public int CustomerId { get; set; }
    [DataMember]
    [ForeignKey("OrderStatus")]
    public int OrderStatusId { get; set; }
    [DataMember]
    public DateTime OrderDate { get; set; }
    [DataMember]
    [NotMapped]
    public decimal ItemsTotal { get { return OrderItems.Count; } }
    [DataMember]
    public ICollection<OrderItem> OrderItems { get; set; }

    public virtual Customer Customer { get; set; }
    public virtual OrderStatus OrderStatus { get; set; }
  }
}
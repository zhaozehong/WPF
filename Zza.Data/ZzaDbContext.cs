using System.Data.Entity;
using Zza.Entities;

namespace Zza.Data
{
  public class ZzaDbContext : DbContext
  {
    public DbSet<Customer> Customers { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<OrderStatus> OrderStatuses { get; set; }

    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
      // TODO ...



      base.OnModelCreating(modelBuilder);
    }
  }
}

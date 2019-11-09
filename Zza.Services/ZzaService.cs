using System;
using System.Linq;
using System.ServiceModel;
using System.Collections.Generic;
using Zza.Data;
using Zza.Entities;

namespace Zza.Services
{
  [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall)]
  public class ZzaService : IZzaService, IDisposable
  {
    public List<Product> GetProducts()
    {
      return _Context.Products.ToList();
    }
    public List<Customer> GetCustomers()
    {
      return _Context.Customers.ToList();
    }
    [OperationBehavior(TransactionScopeRequired = true)]
    public void SubmitOrder(Order order)
    {
      _Context.Orders.Add(order);
      order.OrderItems.ForEach(p => _Context.OrderItems.Add(p));
      _Context.SaveChanges();
    }

    public void Dispose()
    {
      _Context.Dispose();
    }

    private readonly ZzaDbContext _Context = new ZzaDbContext();
  }
}

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
    public ZzaService()
    {
      if(!_Context.Customers.Any())
      {
        var customer = new Customer()
        {
          FirstName = "Zehong",
          LastName = "Zhao",
          Phone = "15317878680",
          Email = "zehong.zhao@hotmail.com",
          Street = "#777 Dushi Rd.",
          City = "Shanghai",
          State = "Shanghai",
          Zip = "200000"
        };
        _Context.Customers.Add(customer);

        var orderStatus = new OrderStatus() { Name = "Deliverying" };
        _Context.OrderStatuses.Add(orderStatus);

        var order = new Order()
        {
          Customer = customer,
          OrderStatus = orderStatus,
          OrderDate = DateTime.Today
        };
        _Context.Orders.Add(order);

        var product1 = new Product()
        {
          Type = "Food",
          Name = "Pizza",
        };
        _Context.Products.Add(product1);
        var product2 = new Product()
        {
          Type = "SoftDrink",
          Name = "Cola",
        };
        _Context.Products.Add(product2);
        var product3 = new Product()
        {
          Type = "Alcohol",
          Name = "X.O",
        };
        _Context.Products.Add(product3);
        var product4 = new Product()
        {
          Type = "Food",
          Name = "Rice",
        };
        _Context.Products.Add(product4);
        var product5 = new Product()
        {
          Type = "Fruit",
          Name = "Apple",
        };
        _Context.Products.Add(product5);

        _Context.SaveChanges();

        var orderItem = new OrderItem()
        {
          Order = order,
          Product = product1,
          Quantity = 1,
          UnitPrice = 5
        };
        _Context.OrderItems.Add(orderItem);
        orderItem = new OrderItem()
        {
          Order = order,
          Product = product5,
          Quantity = 3,
          UnitPrice = 1
        };
        _Context.OrderItems.Add(orderItem);
        orderItem = new OrderItem()
        {
          Order = order,
          Product = product3,
          Quantity = 1,
          UnitPrice = 32
        };
        _Context.OrderItems.Add(orderItem);

        _Context.SaveChanges();
      }
    }
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
      order.OrderItems.ToList().ForEach(p => _Context.OrderItems.Add(p));
      _Context.SaveChanges();
    }

    public void Dispose()
    {
      _Context.Dispose();
    }

    private readonly ZzaDbContext _Context = new ZzaDbContext();
  }
}

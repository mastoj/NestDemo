using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using NestDemo.Model;

namespace NestDemo.CreateIndex
{
    public class CustomerRepository
    {
        public IEnumerable<Customer> GetCustomers()
        {
            var getProductsSql = @"
SELECT c.CustomerID, max(od.UnitPrice) as UnitPrice, sum(od.Quantity) as TotalQuantity, ProductName FROM Customers c
INNER JOIN Orders o ON c.CustomerId = o.CustomerId
INNER JOIN [Order Details] od ON od.OrderID = o.OrderID
INNER JOIN Products p ON p.ProductID = od.ProductID
GROUP BY c.CustomerID, p.ProductName
ORDER BY c.CustomerID

";
            var getCustomersSql = @"
SELECT CustomerID, CompanyName, ContactName, Address, City, Country FROM Customers
";
            var connectionString = @"Data Source=(local)\SQLExpress;Integrated Security=SSPI;Database=NORTHWND";
            using (var connection = new SqlConnection(connectionString))
            {
                try
                {
                    var sets = connection.QueryMultiple(getCustomersSql + getProductsSql);
                    var customers = sets.Read<Customer>().ToDictionary(y => y.CustomerID, y => y);
                    var products = sets.Read((string cId, Product p) =>
                    {
                        customers[cId].Products.Add(p);
                        return p;
                    }, splitOn: "UnitPrice");
                    return customers.Values;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

    }
}
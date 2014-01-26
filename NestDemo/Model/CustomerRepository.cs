using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using Dapper;

namespace NestDemo.Model
{
    public class CustomerRepository
    {
        public IEnumerable<Customer> GetCustomers()
        {
            var getProductsSql = @"
SELECT c.CustomerID, max(od.UnitPrice) as UnitPrice, sum(od.Quantity) as TotalQuantity, p.ProductName, ca.CategoryName FROM Customers c
INNER JOIN Orders o ON c.CustomerId = o.CustomerId
INNER JOIN [Order Details] od ON od.OrderID = o.OrderID
INNER JOIN Products p ON p.ProductID = od.ProductID
INNER JOIN Categories ca ON ca.CategoryID = p.CategoryID
GROUP BY c.CustomerID, p.ProductName, ca.CategoryName
ORDER BY c.CustomerID

";
            var getCustomersSql = @"
SELECT CustomerID, CompanyName, ContactName, Address, City, Country FROM Customers
";
            var connectionString = Settings.NorthwndConnectionString;
            using (var connection = new SqlConnection(connectionString))
            {
                try
                {
                    var sets = connection.QueryMultiple(getCustomersSql + getProductsSql);
                    var customers = sets.Read<Customer>().ToDictionary(y => y.CustomerID, y => y);
                    var customerProdMapping = customers.ToDictionary(y => y.Key, y => new List<Product>());
                    var products = sets.Read((string cId, Product p) =>
                    {
                        customerProdMapping[cId].Add(p);
                        return p;
                    }, splitOn: "UnitPrice");
                    customerProdMapping.ToList().ForEach(y => customers[y.Key].Products = y.Value.ToArray());
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
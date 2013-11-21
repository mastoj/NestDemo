using System.Collections.Generic;

namespace NestDemo.Model
{
    public class Customer
    {
        public Customer()
        {
            Products = new List<Product>();
        }
        public string CustomerID { get; set; }
        public string CompanyName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public List<Product> Products { get; set; }
    }
}
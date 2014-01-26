using Nest;

namespace NestDemo.Model
{
    public class Customer
    {
        public string CustomerID { get; set; }
        public string CompanyName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        [ElasticProperty(Type = FieldType.nested)]
        public Product[] Products { get; set; }
    }
}
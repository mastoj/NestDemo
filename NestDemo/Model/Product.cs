using Nest;

namespace NestDemo.Model
{
    public class Product
    {
        public double UnitPrice { get; set; }
        public int TotalQuantity { get; set; }
        [ElasticProperty(Index = FieldIndexOption.not_analyzed)]
        public string ProductName { get; set; }
        [ElasticProperty(Index = FieldIndexOption.not_analyzed)]
        public string CategoryName { get; set; }
    }
}
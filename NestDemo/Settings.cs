using System.Configuration;

namespace NestDemo
{
    public static class Settings
    {
        public static string Alias
        {
            get
            {
                return "customer_product_mapping";
            }
        }

        public static string ElasticSearchServer
        {
            get
            {
                return ConfigurationManager.AppSettings["ElasticsearchServer"];
            }
        }

        public static string NorthwndConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["northwnd"].ConnectionString;
            }
        }
    }
}
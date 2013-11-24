using System;
using Nest;

namespace NestDemo.ElasticSearch
{
    public class ElasticClientWrapper : ElasticClient
    {
        private static string _connectionString = Settings.ElasticSearchServer;

        private static ConnectionSettings _settings =
            new ConnectionSettings(new Uri(_connectionString)) // http://demoserver:9200
                .SetDefaultIndex(Settings.Alias) // "customer_product_mapping"
                .UsePrettyResponses();

        public ElasticClientWrapper()
            : base(_settings)
        {
        }
    }
}
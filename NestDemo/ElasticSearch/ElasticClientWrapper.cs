using System;
using Nest;

namespace NestDemo.ElasticSearch
{
    public class ElasticClientWrapper : ElasticClient
    {
        private static string _connectionString = "http://demoserver:9200";
        private static string _defaultIndex = "nestdemo";

        private static ConnectionSettings _settings =
            new ConnectionSettings(new Uri(_connectionString))
                .SetDefaultIndex(_defaultIndex)
                .UsePrettyResponses();

        public ElasticClientWrapper()
            : base(_settings)
        {
        }
    }
}
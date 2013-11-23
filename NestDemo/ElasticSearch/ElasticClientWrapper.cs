﻿using System;
using Nest;

namespace NestDemo.ElasticSearch
{
    public class ElasticClientWrapper : ElasticClient
    {
        private static string _connectionString = "http://demoserver:9200";

        private static ConnectionSettings _settings =
            new ConnectionSettings(new Uri(_connectionString))
                .SetDefaultIndex(Settings.Alias)
                .UsePrettyResponses();

        public ElasticClientWrapper()
            : base(_settings)
        {
        }
    }
}
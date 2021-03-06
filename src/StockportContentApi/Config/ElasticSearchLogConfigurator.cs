﻿using System.Linq;
using Elasticsearch.Net;
using Elasticsearch.Net.Aws;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Sinks.Elasticsearch;

namespace StockportContentApi.Config
{
    public class ElasticSearchLogConfigurator
    {
        private const string ElasticsearchConfigurationKey = "ElasticSearchAwsConfiguration";
        private const string ElasticsearchSecretsConfigurationKey = "ElasticSearchAwsSecretsConfiguration";
        private readonly ElasticSearchLogConfiguration _elasticSearchLogConfiguration = new ElasticSearchLogConfiguration();
        private readonly ElasticSearchLogSecretConfiguration _elasticSearchLogSecretConfiguration = new ElasticSearchLogSecretConfiguration();

        public ElasticSearchLogConfigurator(IConfiguration configuration)
        {
            var config = configuration;

            var elasticSearchLogConfigurationSection = config.GetSection(ElasticsearchConfigurationKey);
            var elasticSearchLogSecretConfigurationSection = config.GetSection(ElasticsearchSecretsConfigurationKey);

            if (elasticSearchLogConfigurationSection.AsEnumerable().Any() && elasticSearchLogSecretConfigurationSection.AsEnumerable().Any())
            {
                elasticSearchLogConfigurationSection.Bind(_elasticSearchLogConfiguration);
                elasticSearchLogSecretConfigurationSection.Bind(_elasticSearchLogSecretConfiguration);
            }
            else
            {
                _elasticSearchLogConfiguration.Enabled = false;
            }
        }

        public void Configure(LoggerConfiguration loggerConfiguration)
        {
            if (!_elasticSearchLogConfiguration.Enabled)
            {
                return;
            }

            var options = ElasticSearchLogConfigurator.CreateElasticsearchSinkOptions(_elasticSearchLogConfiguration, _elasticSearchLogSecretConfiguration);
            if (options != null)
            {
                loggerConfiguration.WriteTo.Elasticsearch(options);
            }
        }

        public static ElasticsearchSinkOptions CreateElasticsearchSinkOptions(ElasticSearchLogConfiguration elasticSearchLogConfiguration, ElasticSearchLogSecretConfiguration elasticSearchLogSecretConfiguration)
        {
            if (elasticSearchLogConfiguration == null || elasticSearchLogSecretConfiguration == null)
            {
                return null;
            }

            var singleNodeConnectionPool = new SingleNodeConnectionPool(elasticSearchLogConfiguration.Uri);

            var awsHttpConnection = new AwsHttpConnection(elasticSearchLogConfiguration.Region, new StaticCredentialsProvider(
                new AwsCredentials
                {
                    AccessKey = elasticSearchLogSecretConfiguration.AccessKey,
                    SecretKey = elasticSearchLogSecretConfiguration.SecretKey
                }));

            var options = new ElasticsearchSinkOptions(elasticSearchLogConfiguration.Uri)
            {
                IndexFormat = elasticSearchLogConfiguration.IndexFormat,
                InlineFields = elasticSearchLogConfiguration.InlineFields,
                MinimumLogEventLevel = elasticSearchLogConfiguration.MinimumLogLevel,
                ModifyConnectionSettings = conn => new ConnectionConfiguration(singleNodeConnectionPool, awsHttpConnection)
            };

            return options;
        }
    }
}
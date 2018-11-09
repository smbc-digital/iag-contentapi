﻿using System;
using System.Net;
using System.Threading.Tasks;
using StockportContentApi.Client;
using StockportContentApi.Config;
using Contentful.Core;
using Contentful.Core.Models;
using StockportContentApi.Http;
using Microsoft.Extensions.Logging;

namespace StockportContentApi.Repositories
{
    public class ManagementRepository
    {
        private readonly ContentfulConfig _config;
        private readonly IContentfulManagementClient _client;
        private readonly ILogger<HttpClient> _logger;

        public ManagementRepository(ContentfulConfig config, IContentfulClientManager client, ILogger<HttpClient> logger)
        {
            _config = config;
            _client = client.GetManagementClient(config);
            _logger = logger;
        }

        public async Task<HttpResponse> CreateOrUpdate(dynamic content, SystemProperties systemProperties)
        {
            var entry = new Entry<dynamic>
            {
                Fields = content,
                SystemProperties = systemProperties 
            };

            try
            {
                var group = await _client.CreateOrUpdateEntry(entry, null, null, systemProperties.Version);
                if (group.SystemProperties.Version != null)
                    await _client.PublishEntry(entry.SystemProperties.Id, group.SystemProperties.Version.Value);
                return HttpResponse.Successful(group);
            }
            catch (Exception ex)
            {
                _logger.LogError(new EventId(0), ex, "An unexpected error occured while performing the get operation");
                return HttpResponse.Failure(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        public async Task<HttpResponse> Delete(SystemProperties systemProperties)
        {
            try
            {
                await _client.UnpublishEntry(systemProperties.Id, systemProperties.Version.Value);
                await _client.DeleteEntry(systemProperties.Id, systemProperties.Version.Value);
                return HttpResponse.Successful("Successfully Deleted Entry: " + systemProperties.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(new EventId(0), ex, "An unexpected error occured while performing the get operation");
                return HttpResponse.Failure(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        public async Task<int> GetVersion(string entryId)
        {
            var managementGroup = await _client.GetEntry(entryId);
            return managementGroup.SystemProperties.Version ?? 0;
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using StockportContentApi.Client;
using StockportContentApi.Config;
using Contentful.Core;
using Contentful.Core.Models;
using NLog.Common;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Http;
using StockportContentApi.ManagementModels;

namespace StockportContentApi.Repositories
{
    public class ManagementRepository
    {
        private readonly ContentfulConfig _config;
        private readonly IContentfulManagementClient _client;

        public ManagementRepository(ContentfulConfig config, IContentfulClientManager client)
        {
            _config = config;
            _client = client.GetManagementClient(config);
        }

        public async Task<HttpResponse> CreateOrUpdate(dynamic content, SystemProperties systemProperties = null)
        {
            var entry = new Entry<dynamic>
            {
                Fields = content,
                SystemProperties = systemProperties
            };

            if (systemProperties == null) return HttpResponse.Failure(HttpStatusCode.NotFound, "Not found System Properties");

            var group = await _client.CreateOrUpdateEntryAsync(entry, null, null, systemProperties.Version);

            if (group.SystemProperties.Version != null)
                await _client.PublishEntryAsync(entry.SystemProperties.Id, group.SystemProperties.Version.Value);

            return HttpResponse.Successful(group);
        }

        public async Task<int> GetVersion(string entryId)
        {
            var managementGroup = await _client.GetEntryAsync(entryId);
            if (managementGroup.SystemProperties.Version != null) return managementGroup.SystemProperties.Version.Value;

            return 0;
        }
    }
}
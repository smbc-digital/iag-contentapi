using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contentful.Core;
using Contentful.Core.Errors;
using Contentful.Core.Models;
using Contentful.Core.Search;
using Microsoft.Extensions.Logging;
using StockportContentApi.Client;
using StockportContentApi.Config;
using StockportContentApi.ContentfulModels;

namespace StockportContentApi.Repositories
{
    public interface ISmartResultRepository
    {
        Task<ContentfulSmartResult> Get(string slug);
    }


    public class SmartResultRepository : ISmartResultRepository
    {

        private readonly IContentfulClient _client;
        private readonly ILogger<SmartResultRepository> _logger;

        public SmartResultRepository(ContentfulConfig config, ILogger<SmartResultRepository> logger, IContentfulClientManager contentfulClientManager)
        {
            _logger = logger;
            _client = contentfulClientManager.GetClient(config);
        }

        public async Task<ContentfulSmartResult> Get(string slug)
        {
            try
            {
                var builder = new QueryBuilder<ContentfulSmartResult>()
                    .ContentTypeIs("smartResult")
                    .FieldEquals("fields.slug", slug)
                    .Include(3);

                var entry = await _client.GetEntriesAsync(builder);

                return entry.FirstOrDefault();
            }
            catch (ContentfulException ex)
            {
                _logger.LogWarning(new EventId(), ex, $"There was a problem with getting SmartResult with slug: {slug} from contentful");
                return null;
            }
        }

    }
}

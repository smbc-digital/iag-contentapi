using Contentful.Core;
using StockportContentApi.Client;
using StockportContentApi.Config;
using System.Threading.Tasks;
using Contentful.Core.Errors;
using Contentful.Core.Search;
using Contentful.Core.Models;
using Microsoft.Extensions.Logging;

namespace StockportContentApi.Repositories
{
    public interface IDocumentRepository
    {
        Task<Asset> Get(string assetId);
    }

    public class DocumentRepository : IDocumentRepository
    {
        private readonly IContentfulClient _client;
        private readonly ILogger<DocumentRepository> _logger;

        public DocumentRepository(ContentfulConfig config, IContentfulClientManager contentfulClientManager, ILogger<DocumentRepository> logger)
        {
            _logger = logger;
            _client = contentfulClientManager.GetClient(config);
        }

        public async Task<Asset> Get(string assetId)
        {
            try
            {
                return await _client.GetAssetAsync(assetId, QueryBuilder<Asset>.New);
            }
            catch (ContentfulException ex)
            {
                _logger.LogWarning(new EventId(), ex, $"There was a problem with getting assetId: {assetId} from contentful");
                return null;
            }
        }
    }
}

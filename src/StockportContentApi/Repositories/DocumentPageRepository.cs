using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Contentful.Core.Search;
using StockportContentApi.Config;
using StockportContentApi.Model;
using StockportContentApi.Http;
using StockportContentApi.Utils;
using StockportContentApi.ContentfulFactories;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Client;

namespace StockportContentApi.Repositories
{
    public class DocumentPageRepository : BaseRepository
    {        
        private readonly IContentfulFactory<ContentfulDocumentPage, DocumentPage> _contentfulFactory;
        private readonly DateComparer _dateComparer;
        private readonly ICache _cache;
        private readonly Contentful.Core.IContentfulClient _client;

        public DocumentPageRepository(ContentfulConfig config,
            IContentfulClientManager contentfulClientManager, 
            ITimeProvider timeProvider,
            IContentfulFactory<ContentfulDocumentPage, DocumentPage> contentfulFactory,
            ICache cache)
        {
            _contentfulFactory = contentfulFactory;
            _dateComparer = new DateComparer(timeProvider);
            _client = contentfulClientManager.GetClient(config);
            _cache = cache;
        }

        public async Task<HttpResponse> GetDocumentPage(string documentPageSlug)
        {
            var entry = await _cache.GetFromCacheOrDirectlyAsync("documentPage-" + documentPageSlug, () => GetDocumentPageEntry(documentPageSlug));
            
            var documentPage = entry == null
                ? null
                : _contentfulFactory.ToModel(entry);

            return documentPage == null
                ? HttpResponse.Failure(HttpStatusCode.NotFound, $"No Pdf Page found for '{documentPageSlug}'")
                : HttpResponse.Successful(documentPage);          
        }

        private async Task<ContentfulDocumentPage> GetDocumentPageEntry(string documentPageSlug)
        {
            var builder = new QueryBuilder<ContentfulDocumentPage>()
                .ContentTypeIs("documentPage")
                .FieldEquals("fields.slug", documentPageSlug)
                .Include(3);

            var entries = await _client.GetEntries(builder);

            var entry = entries.FirstOrDefault();
            return entry;
        }
    }
}

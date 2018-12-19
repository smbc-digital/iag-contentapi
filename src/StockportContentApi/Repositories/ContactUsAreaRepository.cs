using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Contentful.Core.Search;
using StockportContentApi.Client;
using StockportContentApi.Config;
using StockportContentApi.ContentfulFactories;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Http;
using StockportContentApi.Model;

namespace StockportContentApi.Repositories
{
    public class ContactUsAreaRepository
    {
        private readonly Contentful.Core.IContentfulClient _client;
        private readonly string _contentType = "footer";
        private readonly IContentfulFactory<ContentfulContactUsArea, ContactUsArea> _contentfulFactory;

        public ContactUsAreaRepository(ContentfulConfig config, IContentfulClientManager clientManager, IContentfulFactory<ContentfulContactUsArea, ContactUsArea> contentfulFactory)
        {
            _client = clientManager.GetClient(config);
            _contentfulFactory = contentfulFactory;
        }

        public async Task<HttpResponse> GetContactUsArea() {

            var builder = new QueryBuilder<ContentfulContactUsArea>().ContentTypeIs("contactUsArea").Include(1);

            var entries = await _client.GetEntries(builder);
            var entry = entries.FirstOrDefault();

            var contactUsArea = _contentfulFactory.ToModel(entry);
            if (contactUsArea == null) return HttpResponse.Failure(HttpStatusCode.NotFound, "No contact us area found");

            return HttpResponse.Successful(contactUsArea);
        }
    }
}

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
    public class ServicePayPaymentRepository
    {
        private readonly Contentful.Core.IContentfulClient _client;
        private readonly IContentfulFactory<ContentfulServicePayPayment, ServicePayPayment> _servicePayPaymentFactory;

        public ServicePayPaymentRepository(ContentfulConfig config,
            IContentfulClientManager clientManager,
            IContentfulFactory<ContentfulServicePayPayment, ServicePayPayment> servicePayPaymentFactory)
        {
            _client = clientManager.GetClient(config);
            _servicePayPaymentFactory = servicePayPaymentFactory;

        }

        public async Task<HttpResponse> GetPayment(string slug)
        {
            var builder = new QueryBuilder<ContentfulServicePayPayment>().ContentTypeIs("servicePayPayment").FieldEquals("fields.slug", slug).Include(1);
            var entries = await _client.GetEntries(builder);
            var entry = entries.FirstOrDefault();

            return entry == null
                ? HttpResponse.Failure(HttpStatusCode.NotFound, $"No service pay payment found for '{slug}'")
                : HttpResponse.Successful(_servicePayPaymentFactory.ToModel(entry));
        }
    }
}

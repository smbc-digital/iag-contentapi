using System.Net;
using System.Threading.Tasks;
using Contentful.Core.Search;
using StockportContentApi.Config;
using StockportContentApi.ContentfulFactories;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Http;
using StockportContentApi.Model;
using System.Linq;
using StockportContentApi.Client;
using StockportContentApi.Factories;

namespace StockportContentApi.Repositories
{
    public class PaymentRepository
    {
        private readonly Contentful.Core.IContentfulClient _client;
        private readonly IContentfulFactory<ContentfulPayment, Payment> _paymentFactory;

        public PaymentRepository(ContentfulConfig config, 
                                 IContentfulClientManager clientManager, 
                                 IContentfulFactory<ContentfulPayment, Payment> paymentFactory)
        {
            _client = clientManager.GetClient(config);
            _paymentFactory = paymentFactory;
        }

        public async Task<HttpResponse> GetPayment(string slug)
        {
            var builder = new QueryBuilder<ContentfulPayment>().ContentTypeIs("payment").FieldEquals("fields.slug", slug).Include(1);
            var entries = await _client.GetEntriesAsync(builder);
            var entry = entries.FirstOrDefault();

            return entry == null 
                ? HttpResponse.Failure(HttpStatusCode.NotFound, $"No payment found for '{slug}'") 
                : HttpResponse.Successful(_paymentFactory.ToModel(entry));
        }
    }
}

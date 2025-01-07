namespace StockportContentApi.Repositories;

public interface IServicePayPaymentRepository
{
    Task<HttpResponse> GetPayment(string slug);
}

public class ServicePayPaymentRepository(ContentfulConfig config,
                                        IContentfulClientManager clientManager,
                                        IContentfulFactory<ContentfulServicePayPayment, ServicePayPayment> servicePayPaymentFactory) : IServicePayPaymentRepository
{
    private readonly IContentfulClient _client = clientManager.GetClient(config);
    private readonly IContentfulFactory<ContentfulServicePayPayment, ServicePayPayment> _servicePayPaymentFactory = servicePayPaymentFactory;

    public async Task<HttpResponse> GetPayment(string slug)
    {
        QueryBuilder<ContentfulServicePayPayment> builder = new QueryBuilder<ContentfulServicePayPayment>().ContentTypeIs("servicePayPayment").FieldEquals("fields.slug", slug).Include(1);
        ContentfulCollection<ContentfulServicePayPayment> entries = await _client.GetEntries(builder);
        ContentfulServicePayPayment entry = entries.FirstOrDefault();

        return entry is null
            ? HttpResponse.Failure(HttpStatusCode.NotFound, $"No service pay payment found for '{slug}'")
            : HttpResponse.Successful(_servicePayPaymentFactory.ToModel(entry));
    }
}

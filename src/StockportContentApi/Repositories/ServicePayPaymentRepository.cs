namespace StockportContentApi.Repositories;

public class ServicePayPaymentRepository
{
    private readonly IContentfulClient _client;
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
        QueryBuilder<ContentfulServicePayPayment> builder = new QueryBuilder<ContentfulServicePayPayment>().ContentTypeIs("servicePayPayment").FieldEquals("fields.slug", slug).Include(1);
        ContentfulCollection<ContentfulServicePayPayment> entries = await _client.GetEntries(builder);
        ContentfulServicePayPayment entry = entries.FirstOrDefault();

        return entry is null
            ? HttpResponse.Failure(HttpStatusCode.NotFound, $"No service pay payment found for '{slug}'")
            : HttpResponse.Successful(_servicePayPaymentFactory.ToModel(entry));
    }
}

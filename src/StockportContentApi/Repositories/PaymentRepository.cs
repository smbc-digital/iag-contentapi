namespace StockportContentApi.Repositories;

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
        var entries = await _client.GetEntries(builder);
        var entry = entries.FirstOrDefault();

        return entry == null
            ? HttpResponse.Failure(HttpStatusCode.NotFound, $"No payment found for '{slug}'")
            : HttpResponse.Successful(_paymentFactory.ToModel(entry));
    }

    public async Task<HttpResponse> Get()
    {
        var builder = new QueryBuilder<ContentfulPayment>().ContentTypeIs("payment").Include(1).Limit(ContentfulQueryValues.LIMIT_MAX);
        var entries = await _client.GetEntries(builder);
        var contentfulPayments = entries as IEnumerable<ContentfulPayment> ?? entries.ToList();

        var payments = GetAllPayments(contentfulPayments);
        return entries == null || !contentfulPayments.Any()
            ? HttpResponse.Failure(HttpStatusCode.NotFound, "No payments found")
            : HttpResponse.Successful(payments);
    }

    private IEnumerable<Payment> GetAllPayments(IEnumerable<ContentfulPayment> entries)
    {
        var entriesList = new List<Payment>();
        foreach (var entry in entries)
        {
            var paymentItem = _paymentFactory.ToModel(entry);
            entriesList.Add(paymentItem);
        }
        return entriesList;
    }
}

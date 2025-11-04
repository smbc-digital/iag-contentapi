namespace StockportContentApi.Repositories;

public interface IPaymentRepository
{
    Task<HttpResponse> GetPayment(string slug, string tagId);
    Task<HttpResponse> Get(string tagId);
}

public class PaymentRepository(ContentfulConfig config,
                            IContentfulClientManager clientManager,
                            IContentfulFactory<ContentfulPayment, Payment> paymentFactory) : IPaymentRepository
{
    private readonly IContentfulClient _client = clientManager.GetClient(config);
    private readonly IContentfulFactory<ContentfulPayment, Payment> _paymentFactory = paymentFactory;

    public async Task<HttpResponse> GetPayment(string slug, string tagId)
    {
        QueryBuilder<ContentfulPayment> builder = new QueryBuilder<ContentfulPayment>()
            .ContentTypeIs("payment")
            .FieldEquals("fields.slug", slug)
            .Include(1);
        
        ContentfulCollection<ContentfulPayment> entries = await _client.GetEntries(builder);
        ContentfulPayment entry = entries.FirstOrDefault();

        return entry is null
            ? HttpResponse.Failure(HttpStatusCode.NotFound, $"No payment found for '{slug}'")
            : HttpResponse.Successful(_paymentFactory.ToModel(entry));
    }

    public async Task<HttpResponse> Get(string tagId)
    {
        QueryBuilder<ContentfulPayment> builder = new QueryBuilder<ContentfulPayment>()
            .ContentTypeIs("payment")
            .Include(1)
            .Limit(ContentfulQueryValues.LIMIT_MAX);
        
        ContentfulCollection<ContentfulPayment> entries = await _client.GetEntries(builder);
        IEnumerable<ContentfulPayment> contentfulPayments = entries as IEnumerable<ContentfulPayment> ?? entries.ToList();
        IEnumerable<Payment> payments = GetAllPayments(contentfulPayments);

        return entries is null || !contentfulPayments.Any()
            ? HttpResponse.Failure(HttpStatusCode.NotFound, "No payments found")
            : HttpResponse.Successful(payments);
    }

    private List<Payment> GetAllPayments(IEnumerable<ContentfulPayment> entries)
    {
        List<Payment> entriesList = new();

        foreach (ContentfulPayment entry in entries)
        {
            Payment paymentItem = _paymentFactory.ToModel(entry);
            entriesList.Add(paymentItem);
        }
        
        return entriesList;
    }
}
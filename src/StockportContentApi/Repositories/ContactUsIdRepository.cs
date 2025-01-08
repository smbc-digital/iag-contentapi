namespace StockportContentApi.Repositories;

public interface IContactUsIdRepository
{
    Task<HttpResponse> GetContactUsIds(string slug);
}

public class ContactUsIdRepository(ContentfulConfig config,
                                IContentfulFactory<ContentfulContactUsId, ContactUsId> contentfulFactory,
                                IContentfulClientManager contentfulClientManager) : IContactUsIdRepository
{
    private readonly IContentfulFactory<ContentfulContactUsId, ContactUsId> _contentfulFactory = contentfulFactory;
    private readonly IContentfulClient _client = contentfulClientManager.GetClient(config);

    public async Task<HttpResponse> GetContactUsIds(string slug)
    {
        QueryBuilder<ContentfulContactUsId> builder = new QueryBuilder<ContentfulContactUsId>().ContentTypeIs("contactUsId").FieldEquals("fields.slug", slug).Include(1);
        ContentfulCollection<ContentfulContactUsId> entries = await _client.GetEntries(builder);
        ContentfulContactUsId entry = entries.FirstOrDefault();

        if (entry is null)
            return HttpResponse.Failure(HttpStatusCode.NotFound, $"No contact us id found for '{slug}'");

        return HttpResponse.Successful(_contentfulFactory.ToModel(entry));
    }
}
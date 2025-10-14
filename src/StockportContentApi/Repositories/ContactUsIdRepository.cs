namespace StockportContentApi.Repositories;

public interface IContactUsIdRepository
{
    Task<HttpResponse> GetContactUsIds(string slug, string tagId);
}

public class ContactUsIdRepository(ContentfulConfig config,
                                IContentfulFactory<ContentfulContactUsId, ContactUsId> contentfulFactory,
                                IContentfulClientManager contentfulClientManager) : IContactUsIdRepository
{
    private readonly IContentfulFactory<ContentfulContactUsId, ContactUsId> _contentfulFactory = contentfulFactory;
    private readonly IContentfulClient _client = contentfulClientManager.GetClient(config);

    public async Task<HttpResponse> GetContactUsIds(string slug, string tagId)
    {
        QueryBuilder<ContentfulContactUsId> builder = new QueryBuilder<ContentfulContactUsId>()
            .ContentTypeIs("contactUsId")
            .FieldEquals("fields.slug", slug)
            .FieldExists("metadata.tags")
            .FieldEquals("metadata.tags.sys.id[in]", tagId)
            .Include(1);
        
        ContentfulCollection<ContentfulContactUsId> entries = await _client.GetEntries(builder);
        ContentfulContactUsId entry = entries.FirstOrDefault();

        if (entry is null)
            return HttpResponse.Failure(HttpStatusCode.NotFound, $"No contact us id found for '{slug}'");

        return HttpResponse.Successful(_contentfulFactory.ToModel(entry));
    }
}